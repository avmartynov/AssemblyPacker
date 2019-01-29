using System.Collections.Generic;
using System.Xml.Serialization;

namespace Twidlle.Infrastructure.Testing
{
    /// <summary> Структура данных конфигурационной секции для работы с набором тестов.</summary> 
    /// <remarks>Работает совместно с классоми TestSet и TestSetAttribute.</remarks>
    public class TestSetSection 
    {
        [XmlAttribute]
        public string BaseDir { get; set; }

        [XmlAttribute]
        public string Config { get; set; }

        [XmlElement("Item")]
        public List<TestItem> Items { get; set; }

        public class TestItem
        {
            [XmlAttribute]
            public string ProjectDir { get; set; }

            [XmlAttribute]
            public string FileName { get; set; }

            [XmlAttribute]
            public string Arguments { get; set; }
        }
    }
}
