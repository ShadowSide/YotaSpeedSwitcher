using System;
using System.Threading.Tasks;

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

    internal class ServiceSettingsLoader : IServiceSettingsSource
    {
        public IServiceSettings Settings => _model;

        private volatile ServiceSettingsModel _model;
    }

    internal class ServiceSettingsModel: IServiceSettings
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public int? MinimalSpeed { get; set; }
        public int? MaximalSpeed { get; set; }
        public int MaximalPrice { get; set; }
        public bool SetFreeSpeedAfterMinimalPrice { get; set; }

        public bool EnabledService { get; set; }
    }
}