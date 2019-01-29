using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class XmlExtensions
    {
        /// <summary> Сериализация объекта в xml строку. </summary>
        [NotNull]
        public static string ToXml<T>([NotNull] this T packet, string nameSpace = "", bool omitXmlDeclaration = true) where T: class
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = omitXmlDeclaration};
            var writer = new StringWriter(sb);
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
                var namespaces = new XmlSerializerNamespaces(new[] {new XmlQualifiedName("", nameSpace)});
                new XmlSerializer(typeof(T)).Serialize(xmlWriter, packet, namespaces);
                return sb.ToString();
            }
        }


        public static T DeserializeXml<T>([NotNull] this string xml, [CanBeNull] string rootElementName = null, [CanBeNull] string defaultNamespace = null)
        {
            if (string.IsNullOrEmpty(rootElementName))
                rootElementName = typeof(T).Name;

            var rootAttribute = new XmlRootAttribute(rootElementName);
            var serializer = new XmlSerializer(typeof(T), null, new Type[0], rootAttribute, defaultNamespace);
            using (var reader = new StringReader(xml))
                return (T)serializer.Deserialize(reader);
        }
    }
}
