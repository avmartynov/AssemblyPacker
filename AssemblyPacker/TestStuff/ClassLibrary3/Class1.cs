using System;

namespace ClassLibrary3
{
    public class Class1
    {
        public Class1()
        {
            var a = new ClassLibrary1.Class1();
            var b = new ClassLibrary2.Class1();

            Console.WriteLine(typeof(Class1).AssemblyQualifiedName);
        }
    }
}
