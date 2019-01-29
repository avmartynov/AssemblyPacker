using System;
using System.Reflection;
using System.Globalization;

namespace PackedAssemblyName
{
    internal class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            Package.Initialize();

            var methodMain = Assembly.Load("AppAssemblyFullName")
                .GetType("StartupTypeName")
                .GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (methodMain == null)
                throw new InvalidOperationException("Type 'StartupTypeName' has not Main method.");

            return (int)(methodMain.Invoke(null, BindingFlags.Static, null, new object[]{args}, CultureInfo.InvariantCulture) ?? 0);
        }
   }
}