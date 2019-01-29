using System;
using System.Data;
using System.Linq.Expressions;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class DataReaderExtensions
    {
        [NotNull]
        public static TTarget ReadValue<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] IDataReader reader,
            [NotNull] Expression<Func<TTarget, TProperty>> property, TProperty defaultValue)
            where TProperty : struct
        {
            return target.Set(property, reader[property.PropertyName()] as TProperty? ?? defaultValue);
        }

        [NotNull]
        public static TTarget ReadValue<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] IDataReader reader,
            [NotNull] Expression<Func<TTarget, TProperty>> property)
            where TProperty : struct
        {
            return target.Set(property,
                reader[property.PropertyName()] as TProperty? 
                ?? throw new InvalidOperationException($"Property '{property.PropertyName()}' can't be NULL."));
        }

        [NotNull]
        public static TTarget Read<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] IDataReader reader,
            [NotNull] Expression<Func<TTarget, TProperty>> property, TProperty defaultValue)
            where TProperty : class
        {
            return target.Set(property, reader[property.PropertyName()] as TProperty ?? defaultValue);
        }

        [NotNull]
        public static TTarget Read<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] IDataReader reader,
            [NotNull] Expression<Func<TTarget, TProperty>> property)
            where TProperty : class
        {
            return target.Set(property, 
                reader[property.PropertyName()] as TProperty 
                ?? throw new InvalidOperationException($"Property '{property.PropertyName()}' can't be NULL."));
        }
    }
}
