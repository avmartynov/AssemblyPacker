using System;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Twidlle.Infrastructure
{
    /// <summary> Типизированная зачитка данных из конфигурационной секции 
    /// с помощью стандартной Xml-сериализации. </summary>
    public static class CustomConfigSection<T>
    {
        /// <summary> Зачитывает данные из конфигурационной секции. </summary>
        /// <param name="sectionName">Имя секции. Если не задано, 
        /// то в качестве имени секции используется имя класса-параметра хранящихся в секции данных.</param>
        /// <returns></returns>
        public static T Read(string sectionName = null)
        {
            if (string.IsNullOrEmpty(sectionName))
                sectionName = typeof(T).Name;

            if (!(ConfigurationManager.GetSection(sectionName) is CustomConfigSection section))
            {
                throw new InvalidOperationException(
                    $"Config section of type {typeof(CustomConfigSection)} named '{sectionName}' is absent.");
            }
            return section.Text.DeserializeXml<T>(rootElementName: sectionName);
        }
    }


    /// <summary> Обеспечивает доступ к тексту секции. </summary>
    /// <inheritdoc/>
    public class CustomConfigSection : ConfigurationSection
    {
        /// <inheritdoc/>
        protected override void DeserializeSection(XmlReader reader)
        {
            reader.Read();
            using (var sr = new StringReader(reader.ReadOuterXml()))
                Text = sr.ReadToEnd();
        }

        /// <summary> Текст секции </summary>
        public string Text { get; private set; }
    }
}