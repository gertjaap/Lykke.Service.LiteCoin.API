using Autofac;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Binder
{
    public  class ServiceModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinAPISettings> _settings;
        public ServiceModule(IReloadingManager<LiteCoinAPISettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterNetwork(builder);
            //builder.RegisterInstance(new InsightsApiSettings
            //{
            //    Url = _settings.CurrentValue.InsightAPIUrl
            //});
        }

        private void RegisterNetwork(ContainerBuilder builder)
        {
            NBitcoin.Litecoin.Networks.Register();
            builder.RegisterInstance(Network.GetNetwork(_settings.CurrentValue.Network)).As<Network>();


        }
    }
}
