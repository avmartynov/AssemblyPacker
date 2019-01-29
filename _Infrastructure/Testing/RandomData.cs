using System;
using System.Linq;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Процедуры для генерирования случайных тестовых данных. </summary>
    public static class RandomData
    {
        /// <summary> Возвращает случайное целое число - идентификатор из заданного диапазона. </summary>
        /// TODO: Нужен случайный long.
        public static int GenerateRandomId(int maxValue, int minValue = 0)
        {
            return _rand.Next(minValue, maxValue);
        }


        /// <summary> Возвращает случайную строку символов заданной длинны. </summary>
        /// <param name="size">Размер строки.</param>
        [NotNull]
        public static string GenerateRandomSymbolString(int size)
        {
            return new string(Enumerable.Range(0, size)
                             .Select(i => GenerateRandomSymbol())
                             .ToArray());
        }


        /// <summary> Возвращает случайную строку цифр заданной длинны. </summary>
        /// <param name="size">Размер строки.</param>
        [NotNull]
        public static string GenerateRandomDigitString(int size)
        {
            return new string(Enumerable.Range(0, size)
                             .Select(i => GenerateRandomDigit())
                             .ToArray());
        }


        /// <summary> Возвращает случайный символ. </summary>
        public static char GenerateRandomSymbol()
        {
            var minValue  = char.ConvertToUtf32(" ", 0);
            var maxValue  = char.ConvertToUtf32("~", 0);
            var utf32Code = _rand.Next(minValue, maxValue);
            var s         = char.ConvertFromUtf32(utf32Code);
            return s[0];
        }


        /// <summary> Возвращает случайную цифру. </summary>
        public static char GenerateRandomDigit()
        {
            var minValue  = char.ConvertToUtf32("0", 0);
            var maxValue  = char.ConvertToUtf32("9", 0);
            var utf32Code = _rand.Next(minValue, maxValue);
            var s         = char.ConvertFromUtf32(utf32Code);
            return s[0];
        }

        private static readonly Random _rand = new Random();
    }

}
