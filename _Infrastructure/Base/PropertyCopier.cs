using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;
using static System.Linq.Expressions.Expression;

namespace Twidlle.Infrastructure
{
    public static class PropertyCopier
    {
        /// <summary> Скопировать значения одноимённых свойств. </summary>
        [NotNull] [Pure]
        public static TTarget CopyTo<TSource, TTarget>([NotNull] this TSource source, [NotNull] TTarget target)
        {
            PropertyCopier<TSource, TTarget>.CopyTo(source, target);
            return target;
        }

        /// <summary> Скопировать значения одноимённых свойств. </summary>
        [NotNull] [Pure]
        public static TTarget CopyFrom<TSource, TTarget>([NotNull] this TTarget target, [NotNull] TSource source)
        {
            PropertyCopier<TSource, TTarget>.CopyTo(source, target);
            return target;
        }
    }

    public static class PropertyCopier<TSource, TTarget>
    {
        /// <summary> Скопировать значения одноимённых свойств. </summary>
        public static TTarget CopyTo(TSource source, TTarget target)
        {
            _copyProcedure.Value(target, source);
            return target;
        }

        private static Action<TTarget, TSource> CreateCopyProcedure()
        {
            var targetProperties = typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var sourceProperties = typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var targetVariable = Variable(typeof(TTarget));
            var sourceVariable = Variable(typeof(TSource));
            
            var copyProcs = targetProperties.Select(targetProperty => GetCallSetMethodOrDefault(targetProperty, sourceProperties, targetVariable, sourceVariable))
                            .Where(callingSetMethod => callingSetMethod != null)
                            .ToList();
            if (!copyProcs.Any())
                return (t, s) => { };

            var block = Block(copyProcs);
            var lambda = Lambda(block, targetVariable, sourceVariable);
            var expression = (Expression<Action<TTarget, TSource>>) lambda;

            return expression.Compile();
        }

        private static MethodCallExpression GetCallSetMethodOrDefault(
            PropertyInfo              targetProperty, 
            IEnumerable<PropertyInfo> sourceProperties,
            Expression                targetVariable,
            Expression                sourceVariable
            )
        {
            var setMethod = targetProperty.GetSetMethod();
            if (setMethod == null)
                return null;
            
            var matchedSourceProperty = sourceProperties.FirstOrDefault(sourceProperty => sourceProperty.IsMatch(targetProperty));
            var getMethod = matchedSourceProperty?.GetGetMethod();
            if (getMethod == null)
                return null;
            
            var callGetMethod = Call(sourceVariable, getMethod);
            return targetProperty.PropertyType == matchedSourceProperty.PropertyType
                ? Call(targetVariable, setMethod, callGetMethod)
                : Call(targetVariable, setMethod, Convert(callGetMethod, targetProperty.PropertyType));
        }

        private static readonly Lazy<Action<TTarget, TSource>> _copyProcedure = 
                            new Lazy<Action<TTarget, TSource>>(CreateCopyProcedure, isThreadSafe: true);
    }

    internal static class PropertyCopierExtensions
    {
        public static bool IsMatch(this PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            if (String.Compare(sourceProperty.Name, targetProperty.Name, StringComparison.OrdinalIgnoreCase) != 0)
                return false;

            var isMatch = targetProperty.PropertyType.IsAssignableFrom (sourceProperty.PropertyType) 
                       || targetProperty.PropertyType.IsConvertibleFrom(sourceProperty.PropertyType);

            if (!isMatch)
            {
                _logger.TraceEvent(TraceEventType.Warning, 0, "Propery {0}.{1} is not convertible from {2}.{1}.",
                    sourceProperty.DeclaringType, sourceProperty.Name, targetProperty.DeclaringType);
            }

            return isMatch;
        }

        private static bool IsConvertibleFrom(this Type targetPropertyType, Type sourcePropertyType)
        {
            var sourceVariable = Variable(sourcePropertyType);
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert(sourceVariable, targetPropertyType);
                return true;
            }
            catch (Exception x)
            {
                _logger.TraceEvent(TraceEventType.Warning, 0, x.Message);
                return false;
            }
        }

        private static readonly TraceSource _logger = new TraceSource("Twidlle.Infrastructure", SourceLevels.Warning);
    }
}
