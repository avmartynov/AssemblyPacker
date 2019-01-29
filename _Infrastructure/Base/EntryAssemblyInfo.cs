using System;
using System.Linq;
using System.Reflection;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary>
    /// Простой доступ к свойства сборки-приложения (сборки, сожержащей точку входа приложения). 
    /// Этот код не работает, если если его вызывать из Web-приложения (Там не .Net-овская точка входа).
    /// </summary>
    public static class EntryAssemblyInfo
    {
        /// <summary> Gets the simple name of the assembly </summary>
        public static string Name     
            => AttributedAssembly.GetName().Name;

        /// <summary> Gets the major, minor, build, and revision numbers of the entry assembly as string. </summary>
        [NotNull]
        public static string Version  
            => AttributedAssembly.GetName().Version.ToString();

        /// <summary> Provides a Descriptiont attribute value for an entry assembly. </summary>
        [NotNull]
        public static string Description   
            => GetAttributeOrDefault<AssemblyDescriptionAttribute>()?.Description ?? "";

        /// <summary> Provides a Product attribute value for an entry assembly. </summary>
        [NotNull]
        public static string Product       
            => GetAttributeOrDefault<AssemblyProductAttribute>()?.Product ?? "";

        /// <summary> Provides a Configuration attribute value for an entry assembly. </summary>
        [NotNull]
        public static string Configuration 
            => GetAttributeOrDefault<AssemblyConfigurationAttribute>()?.Configuration ?? "";

        /// <summary> Provides a Copyright attribute value for an entry assembly. </summary>
        [NotNull]
        public static string Copyright     
            => GetAttributeOrDefault<AssemblyCopyrightAttribute>()?.Copyright ?? "";

        /// <summary> Provides a Company attribute value for an entry assembly. </summary>
        [NotNull]
        public static string Company       
            => GetAttributeOrDefault<AssemblyCompanyAttribute>()?.Company ?? "";

        /// <summary> Provides a Title attribute value for an entry assembly. </summary>
        public static string Title         
            => GetAttributeOrDefault<AssemblyTitleAttribute>()?.Title ?? Name;

        /// <summary> Задаёт то, атрибуты какой сборки используются как атрибуты приложения </summary>
        public static void SetAttributedAssembly([CanBeNull] Assembly assembly = null)
            => _attributeAssembly = assembly ?? Assembly.GetCallingAssembly();

        #region Private members

        [NotNull]
        private static Assembly AttributedAssembly
            => _attributeAssembly ?? Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("There is no entry assembly.");

        [CanBeNull]
        private static T GetAttributeOrDefault<T>() where T : Attribute
            => AttributedAssembly.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();

        private static Assembly _attributeAssembly;

        #endregion
    }
}
