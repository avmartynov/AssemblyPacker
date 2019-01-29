using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class XmlFile
    {
        public static T Read<T>([NotNull] string filePath, [CanBeNull] string rootTag = null)
        {
            var root = new XmlRootAttribute(rootTag ?? GetDefaultRootTag<T>());
            var xr = new XmlSerializer(typeof(T), root);
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                return (T)xr.Deserialize(fs);
        }


        public static void Write<T>([NotNull] T source, [NotNull] string filePath, [CanBeNull] string rootTag = null)
        {
            var root = new XmlRootAttribute(rootTag ?? GetDefaultRootTag<T>());
            var xr = new XmlSerializer(typeof(T), root);
            using (var writer = new StreamWriter(filePath))
                xr.Serialize(writer, source);
        }


        [NotNull]
        private static string GetDefaultRootTag<T>()
        {
            return typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), inherit: true)
                                .Cast<XmlRootAttribute>()
                                .SingleOrDefault()
                                ?.ElementName
                                ?? typeof(T).Name;
        }
    }
}
