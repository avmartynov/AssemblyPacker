using System.Linq;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure.WinForms
{                    
    public class EnumItem
    {
        public EnumItem(int val, string name)
        {
            IntegerValue = val;
            LocalName = name;
        }

        public int    IntegerValue { get; }
        public string LocalName  { get; }  

        [NotNull]
        public static EnumItem[] SplitEnumItems([NotNull] string items)
            => items.SplitLines().Select((n, v) => new EnumItem(v, n)).ToArray();
    }
}
