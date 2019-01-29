using System;
using System.Configuration.Install;
using System.ComponentModel;

namespace PackedAssemblyName
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            foreach (var typeName in "InstallerTypeNames".Split(';'))
            {
                var type = Type.GetType(typeName, throwOnError: true);
                var installer = (Installer)Activator.CreateInstance(type);
                Installers.Add(installer);
            }
        }
    }
}
