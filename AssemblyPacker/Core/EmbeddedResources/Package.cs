using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace PackedAssemblyName
{
    public static class Package
    {
        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) => Resolve(a.Name);
        }


        private static Assembly Resolve(string assemblyName)
        {
            if (_assemblies.ContainsKey(assemblyName))
                return _assemblies[assemblyName];

            string resourceName;
            var resource = GetResource("PackedAssembly.dll." + assemblyName) ?? 
                           GetResource("PackedAssembly.exe." + assemblyName);
            if (resource == null && _resources.TryGetValue(assemblyName, out resourceName))
            {
                resource = GetResource(resourceName);
                assemblyName = GetAssemblyName(resourceName);
                if (_assemblies.ContainsKey(assemblyName))
                    return _assemblies[assemblyName];
            }
            if (resource == null)
                return null;

            using (var r = new BinaryReader(resource))
            using (var d = new MemoryStream())
            using (var g = new GZipStream(new MemoryStream(r.ReadBytes((int) resource.Length)), CompressionMode.Decompress))
            {
                g.CopyTo(d);
                return _assemblies[assemblyName] = Assembly.Load(d.GetBuffer());
            }
        }


        private static Stream GetResource(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }


        private static string GetAssemblyName(string resourceName)
        {
            return resourceName.Substring("PackedAssembly.dll.".Length);
        }


        private static readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        private static readonly Dictionary<string, string> _resources =
            Assembly.GetExecutingAssembly().GetManifestResourceNames()
                    .Select(resourceName => new
                                       {
                                          new AssemblyName(GetAssemblyName(resourceName)).Name,
                                          resourceName
                                       })
                    .GroupBy(i => i.Name)
                    .Select(g => g.First())
                    .ToDictionary(i => i.Name, i => i.resourceName);
    }
}