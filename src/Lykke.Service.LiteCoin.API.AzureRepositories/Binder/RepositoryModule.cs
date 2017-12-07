using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.TxTracker;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Binder
{
    public  class RepositoryModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<LiteCoinAPISettings> _settings;
        public RepositoryModule(IReloadingManager<LiteCoinAPISettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterRepo(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new LastProcessedBlockRepository(
                AzureTableStorage<LastProcessedBlockEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "LastProcessedBlocks", _log)));
        }
    }
}
