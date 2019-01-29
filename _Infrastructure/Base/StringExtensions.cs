using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class StringExtensions
    {
        [NotNull]
        public static string Format([NotNull] this string source, [NotNull] params object[] args)
            => String.Format(source, args);


        [NotNull]
        public static string Join([NotNull] this IEnumerable<string> source, string separator)
            => String.Join(separator, source);


        [NotNull]
        public static IEnumerable<string> SplitLines([NotNull] this string source, [CanBeNull] string separator = null, 
                                                      StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Split(new[] {separator ?? Environment.NewLine}, options);
        }

        [NotNull]
        public static String JoinLines([NotNull] this String text, string separator = " ")
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return String.Join(separator, text.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
        }


        public static bool Match([NotNull] this string source, [NotNull] string regexp)
            => new Regex(regexp).Match(source).Success;


        public static TEnum ParseEnumName<TEnum>([NotNull] this string enumName) where TEnum : struct 
            => (TEnum)Enum.Parse(typeof(TEnum), value: enumName, ignoreCase: true);

        [NotNull]
        public static string PathCanonicalize([NotNull] this string source)
            => new FileInfo(source).FullName;


        [NotNull]
        public static IDictionary<string, string> ToDictionary([NotNull] this NameValueCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var dict = new Dictionary<string, string>();
            Array.ForEach(collection.AllKeys, key => dict.Add(key, collection[key]) );
            return dict;
        }


        [NotNull]
        public static string ToJson(this object obj)
            => _jsonSerializer.Serialize(obj);


        public static T DeserializeJson<T>([NotNull] this string s)
            => _jsonSerializer.Deserialize<T>(s);


        private static readonly JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();
    }
}
