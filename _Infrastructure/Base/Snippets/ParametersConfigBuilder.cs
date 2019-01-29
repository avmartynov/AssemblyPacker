using Microsoft.Configuration.ConfigurationBuilders;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    /// <summary>
    /// Позволяет ссылаться на значения AppSettings из в других частей конфигурационного файла,
    /// т.е. использовать AppSettings как переменные.
    /// </summary>
    public class ParametersConfigBuilder : KeyValueConfigBuilder
    {
        [NotNull]
        public override string GetValue([NotNull] string key) 
            => (ConfigurationManager.GetSection("parameters") as NameValueCollection)?[key] ?? string.Empty;

        [NotNull]
        public override ICollection<KeyValuePair<string, string>> GetAllValues(string prefix)
        {
            var settings = (ConfigurationManager.GetSection("ConfigParameters") as NameValueCollection);
            return settings?.AllKeys.Where(key => key.StartsWith(prefix))
                            .ToDictionary(key => key, key => settings[key])  ?? new Dictionary<string, string>();
        }
    }
}
/*  --------------------------------------------------------------------------------

1. Объявить и добавить секцию с параметрами конфигурации типа AppSettingsSection
2. Объявить и добавить стандарттую секцию configBuilders типа ConfigurationBuildersSection
3. Добавить билдер типа ParametersConfigBuilder
4. Сослаться на типа ParametersConfigBuilder в аттрибуте configBuilders элемента конфигурации.
5. В атрибутах и содержимом элемента конфигурации можно ссылаться на параметры как ${MyName}

[Построители конфигурации для ASP.NET] (https://docs.microsoft.com/ru-ru/aspnet/config-builder#additional-resources )

<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="parameters"     type="System.Configuration.AppSettingsSection, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
    <section name="configBuilders" type="System.Configuration.ConfigurationBuildersSection, System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" restartOnExternalChanges="false" requirePermission="false" />
    <section name="MyConfig"       type="Twidlle.Infrastructure.CustomConfigSection, Twidlle.Infrastructure.Base"/>
  </configSections>

  <configBuilders>
    <builders>
      <add name="Parameters" mode="Expand" type="ConsoleApp2.ParametersConfigBuilder, ConsoleApp2" />

      <add name="AS_Environment" mode="Greedy" prefix="AppSettings_" stripPrefix="true"
           type="Microsoft.Configuration.ConfigurationBuilders.EnvironmentConfigBuilder, 
           Microsoft.Configuration.ConfigurationBuilders.Environment" />

      <add name="CS_Environment" mode="Greedy" prefix="ConnectionStrings_" stripPrefix="true"
           type="Microsoft.Configuration.ConfigurationBuilders.EnvironmentConfigBuilder, 
           Microsoft.Configuration.ConfigurationBuilders.Environment" />

      <add name="Expand_Environment" mode="Expand"
           type="Microsoft.Configuration.ConfigurationBuilders.EnvironmentConfigBuilder, 
           Microsoft.Configuration.ConfigurationBuilders.Environment" />
    </builders>
  </configBuilders>

  <parameters>
    <add key="A" value="abc" />
    <add key="Count" value="999" />
    <add key="MyName" value="AVM" />
  </parameters>

  <MyConfig configBuilders="Parameters">
    <Count>${Count}</Count>
    <Name>${MyName}</Name>
  </MyConfig>

  <appSettings configBuilders="AS_Environment, Expand_Environment, Parameters">
    <add key="ServiceID" value="ServiceID value from web.config" />
    <add key="ServiceKey" value="ServiceKey value from web.config" />
    <add key="ServiceExp" value="windir=${windir}" />
    <add key="ServiceExp2" value="A=${A}" />
  </appSettings>


  <connectionStrings configBuilders="CS_Environment, Expand_Environment, Parameters">
    <add name="Default" connectionString="Data Source=web.config/mydb.db" />
  </connectionStrings>

  <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
    </startup>
</configuration>

*/
