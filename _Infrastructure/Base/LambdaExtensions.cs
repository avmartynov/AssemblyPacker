using System;
using System.Linq.Expressions;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary> Методы-разширения для работы со свойствами в типизированном виде (с их лямбдами) </summary>
    public static class LambdaExtensions
    {
        /// <summary> Устанавливает новое значение свойства по его лямбде. </summary>
        /// <typeparam name="TTarget"> Тип объекта - владельца свойства </typeparam>
        /// <typeparam name="TProperty"> Тип свойства </typeparam>
        /// <param name="target"> Объект - владелец свойства</param>
        /// <param name="property"> Свойство в виде лямбды </param>
        /// <param name="value"> Новое значение свойства </param>
        [NotNull] 
        public static TTarget Set<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] Expression<Func<TTarget, TProperty>> property, TProperty value)
        {
            GetPropertyInfo(property).SetValue(target, value);
            return target;
        }

        /// <summary> Возвращает значение свойства по его лямбде. </summary>
        /// <typeparam name="TTarget"> Тип объекта - владельца свойства </typeparam>
        /// <typeparam name="TProperty"> Тип свойства </typeparam>
        /// <param name="target"> Объект - владелец свойства</param>
        /// <param name="property"> Свойство в виде лямбды </param>
        public static TProperty Get<TTarget, TProperty>([NotNull] this TTarget target, [NotNull] Expression<Func<TTarget, TProperty>> property)
            => (TProperty)GetPropertyInfo(property).GetValue(target);

        /// <summary> Возвращает имя свойства по его лямбде </summary>
        /// <typeparam name="TTarget"> Тип объекта - владельца свойства.</typeparam>
        /// <typeparam name="TProperty">Тип свойства</typeparam>
        /// <param name="property">Свойство в виде лямбды</param>
        [NotNull] 
        public static string PropertyName<TTarget, TProperty>([NotNull] this Expression<Func<TTarget, TProperty>> property)
            => GetPropertyInfo(property).Name;

        #region Private members

        [NotNull] 
        private static PropertyInfo GetPropertyInfo<TTarget, TProperty>([NotNull] Expression<Func<TTarget, TProperty>> property)
            => (PropertyInfo)((MemberExpression) (property  ?? throw new ArgumentNullException(nameof(property))).Body).Member;

        #endregion Private members
    }
}
