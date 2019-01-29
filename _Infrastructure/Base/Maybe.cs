using System;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary> Полезняшки для удобства программирования в функциональном стиле. </summary>
    public static class MaybeExtensions
    {
        /// <summary> Прерывает цепочку вызовов методов, если предикат возвращает true. </summary>
        [CanBeNull]
        public static T DefaultIf<T>([NotNull] this T obj, [NotNull] Predicate<T> condition)
            => condition(obj) ? default : obj;


        /// <summary> Прерывает цепочку вызовов методов, если условие выполняется. </summary>
        [CanBeNull]
        public static T DefaultIf<T>([NotNull] this T obj, bool condition)
            => condition ? default : obj;


        /// <summary> Прерывает цепочку вызовов методов, если предикат возвращает false. </summary>
        [CanBeNull]
        public static T DefaultIfNot<T>([NotNull] this T obj, [NotNull] Predicate<T> condition)
            => condition(obj) ? obj : default;


        /// <summary> Прерывает цепочку вызовов методов, если условие не выполняется. </summary>
        [CanBeNull]
        public static T DefaultIfNot<T>([NotNull] this T obj, bool condition)
            => condition ? obj : default;


        /// <summary> Для типа-значения заменяет его дефолтное значение на null. </summary>
        public static TInput? DefaultAsNull<TInput>(this TInput obj) where TInput : struct
            => obj.Equals(default(TInput)) ? new TInput?() : obj;


        public static TInput DefaultAs<TInput>(this TInput obj, TInput defaultValue) where TInput : struct
            => obj.Equals(default(TInput)) ? defaultValue : obj;


        public static T? ToNullable<T>(this T v, bool specified) where T : struct
            => specified ? v : default(T?);


        /// <summary>  </summary>
        public static T Call<T>(this T obj, [NotNull] Action<T> action)
        {
            action(obj);
            return obj;
        }


        /// <summary>  </summary>
        public static T CallIf<T>(this T obj, bool condition, [NotNull] Func<T, T> func)
            => condition ? func(obj): obj;


        /// <summary>  </summary>
        public static T CallIf<T>(this T obj, [NotNull] Func<bool> predicate, [NotNull] Func<T, T> func)
            => obj.CallIf(predicate(), func);
    }
}