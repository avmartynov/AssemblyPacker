using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.Diagnostics
{
    using Formatter = Func<object, string>;

    /// <summary> Класс-процедура преобразования данных в JSON5 формат. </summary>
    public static class Json5
    {
        /// <summary> Формат данных для протоколирования и отладки (в одну строку). </summary>
        public static string ToJson5(this object x)
            => _formatter.Value(x);

        /// <summary> Формат данных для чтения человеком (многострочный, с отступами). </summary>
        public static string ToIndentedJson5(this object x)
            => _indentedFormatter.Value(x);

        #region Private members    

        [NotNull]
        private static Formatter CreateFormatter()
            => CreateFormatter(indented: false);

        [NotNull]
        private static Formatter CreateIndentedFormatter()
            => CreateFormatter(indented: true);

        [NotNull]
        private static Formatter CreateFormatter(bool indented)
        {
            var stringWriter  = new StringWriter();
            var textWriter    = new MyJsonTextWriter(stringWriter, indented) {QuoteName = false};
            var stringBuilder = stringWriter.GetStringBuilder();
            var serializer    = JsonSerializer.Create(new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});

            return obj =>
                   {
                       stringBuilder.Clear();
                       serializer.Serialize(textWriter, obj);
                       return stringBuilder.ToString();
                   };
        }

        /// <inheritdoc />
        /// <summary> Класс-наследник JsonTextWriter, для улучшения читаемости JSON5 формата. </summary>
        public sealed class MyJsonTextWriter : JsonTextWriter
        {
            public MyJsonTextWriter(TextWriter textWriter, bool indented = false) : base(textWriter)
            {
                Formatting = indented ? Formatting.Indented : Formatting.None;
                Indentation = 4;
                IndentChar = ' ';
            }

            protected override void WriteValueDelimiter()
            {
                base.WriteValueDelimiter();

                // Вставляем пробел после запятой, разделяющей поля.
                WriteWhitespace(" "); 
            }

            public override void WritePropertyName([NotNull] string name, bool escape)
            {
                // Свойства пишем с заглавной буквы.
                base.WritePropertyName(Capitalize(name), escape);

                // Вставляем пробел после двоеточия.
                WriteWhitespace(" "); 
            }

            [NotNull]
            private static string Capitalize([NotNull] string s)
                => Char.IsUpper(s[0]) ? s : s.Remove(0, 1).Insert(0, s.Substring(0, 1).ToUpper());
        }

        private static readonly ThreadLocal<Formatter> _formatter         = new ThreadLocal<Formatter>(CreateFormatter);
        private static readonly ThreadLocal<Formatter> _indentedFormatter = new ThreadLocal<Formatter>(CreateIndentedFormatter);

        #endregion Private members
    }
}