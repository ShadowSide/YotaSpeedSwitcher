using Common;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace YotaSpeedSwitcherService
{
    internal interface IServiceSettings
    {
        string Login { get; }
        string Password { get; }

        int? MinimalSpeed { get; }
        int? MaximalSpeed { get; }
        int MaximalPrice { get; }
        bool SetFreeSpeedAfterMinimalPrice { get; }

        bool EnabledService { get; }
    }

    internal interface IServiceSettingsSource
    {
        IServiceSettings Settings { get; }
    }

    internal class ServiceSettingsLoader : AutostartPeriodicActionTask, IServiceSettingsSource
    {
        public ServiceSettingsLoader() : 
            base(LoadSettings, TimeSpan.FromSeconds(10), true)
        {
        }

        private void LoadSettings()
        {
            var filePath = Path.Combine(EnvironmentSettings.ApplicationDataDirectory, ConfigFileName);
            if (!File.Exists(filePath))
            {
                LoadDefaultsAndSave(filePath);
                return;
            }
            using (var stream = File.OpenRead(filePath))
            using (var textReader = new StreamReader(stream, Encoding.UTF8))
            {
                var nextSettings = (ServiceSettingsModel)JsonConvert.DeserializeObject(textReader.ReadToEnd(), typeof(ServiceSettingsModel));
                _model = nextSettings;
            }
        }

        private void LoadDefaultsAndSave(string filePath)
        {
            try
            {
                var nextSettings = (ServiceSettingsModel)JsonConvert.DeserializeObject("{}", typeof(ServiceSettingsModel));
                _model = nextSettings;
                using (var stream = File.OpenWrite(filePath))
                using (var textWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    var nextSettingsString = JsonConvert.SerializeObject(nextSettings);
                    textWriter.Write(nextSettingsString);
                    textWriter.Flush();
                }
            }
            catch
            {

            }
        }

        public IServiceSettings Settings => _model;

        private const string ConfigFileName = "YotaSpeedSwitcher.json";
        private volatile ServiceSettingsModel _model;
    }

    internal class ServiceSettingsModel: IServiceSettings
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public int? MinimalSpeed { get; set; }
        public int? MaximalSpeed { get; set; }

        [DefaultValue(1400)]
        public int MaximalPrice { get; set; }
        [DefaultValue(true)]
        public bool SetFreeSpeedAfterMinimalPrice { get; set; }

        [DefaultValue(true)]
        public bool EnabledService { get; set; }
    }
}