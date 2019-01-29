    using System;
using System.Reflection;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Проверяем работоспособность приложения-пакета. Взможность работы с отражением. 
            // Единственно, что нельзя делать - предполагать, что сборка лежить в файле 
            // на диске по конкреному пути.
            try
            {
                Console.WriteLine("1. Динамическая загрузка типа по частичному имени");

                const String partialTypeName = "ClassLibrary2.Class1, ClassLibrary2";
                var type = Type.GetType(partialTypeName);
                if (type == null)
                    throw new InvalidOperationException(string.Format("Can't load type '{0}'.", partialTypeName));

                Console.WriteLine("\tТип {0} динамически загружен по частичному имени.", type.AssemblyQualifiedName);

                // ////////////////////////

                Console.WriteLine("2. Динамическая сборки по частичному имени");
                
                const String partialAsmName = "ClassLibrary2";
                var asm = Assembly.Load(partialAsmName);
                if (asm == null)
                    throw new InvalidOperationException(string.Format("Can't load assembly '{0}'.", partialAsmName));

                Console.WriteLine("\tСборка {0} динамически загружена по частичному имени.", asm);

                // ////////////////////////

                Console.WriteLine("3. Динамическая загрузка типа по частичному имени");

                type = asm.GetType("ClassLibrary2.Class1");
                if (type == null)
                    throw new InvalidOperationException(string.Format("Can't load type '{0}'.", partialTypeName));

                Console.WriteLine("\tТип {0} динамически загружен по частичному имени.", type.AssemblyQualifiedName);
                
                // ////////////////////////

                Console.WriteLine("4. Динамическая загрузка подписанной сборки по полному имени");

                const String fullAsmName = "ClassLibrary2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3faef1259cb2e7bf";
                asm = Assembly.Load(fullAsmName);
                if (asm == null)
                    throw new InvalidOperationException(string.Format("Can't load assembly '{0}'.", fullAsmName));

                Console.WriteLine("\tСборка {0} динамически загружена по полному имени.", asm);

                // ////////////////////////

                Console.WriteLine("5. Вызов статически линкованного кода");

                var c1 = new ClassLibrary1.Class1();

                Console.WriteLine("\tТип ClassLibrary1.Class1 используется по статической ссылке.");

                // ////////////////////////

                Console.WriteLine("Press any key to continue...");
            }
            catch (Exception x)
            {
                Console.WriteLine(x);    
            }
            Console.ReadLine();
        }
    }
}
