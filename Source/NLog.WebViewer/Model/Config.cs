using System;
using System.Configuration;

namespace NLog.WebViewer
{
    public sealed class Config : ConfigurationSection
    {
        private static readonly Lazy<Config> LazyInstance = new Lazy<Config>(GetConfig);

        private string[] _fieldsList;

        public static Config Instance
        {
            get { return LazyInstance.Value; }
        }

        [ConfigurationProperty("Separator", IsRequired = false, DefaultValue = "|")]
        public string Separator
        {
            get { return (string)this["Separator"]; }
        }

        [ConfigurationProperty("Fields", IsRequired = true)]
        public string Fields
        {
            get { return (string)this["Fields"]; }
        }

        [ConfigurationProperty("FileFormat", IsRequired = false, DefaultValue = "{0:yyyy-MM-dd}.txt")]
        public string FileFormat
        {
            get { return (string)this["FileFormat"]; }
        }

        [ConfigurationProperty("Path", IsRequired = false, DefaultValue = "~/App_Data/Logs")]
        public string Path
        {
            get { return (string)this["Path"]; }
        }

        public string[] FieldsList
        {
            get
            {
                return _fieldsList ?? (_fieldsList = Fields.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public bool DefaultConfig { get; private set; }

        private static Config GetConfig()
        {
            const string ConfigSection = "nlog.webviewer";

            return (Config)ConfigurationManager.GetSection(ConfigSection) ?? new Config { DefaultConfig = true };
        }
    }
}