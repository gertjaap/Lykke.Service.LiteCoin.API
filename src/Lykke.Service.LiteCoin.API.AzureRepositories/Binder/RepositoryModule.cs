using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.CashOut;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations;
using Lykke.Service.LiteCoin.API.AzureRepositories.Queue;
using Lykke.Service.LiteCoin.API.AzureRepositories.TxTracker;
using Lykke.Service.LiteCoin.API.AzureRepositories.WebHook;
using Lykke.Service.LiteCoin.API.Core.BlockChainTracker;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.WebHook;
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
            RegisterQueue(builder);
        }

        private void RegisterRepo(ContainerBuilder builder)
        {
            builder.RegisterInstance(new LastTrackedBlockRepository(
                AzureTableStorage<LastProcessedBlockEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "LastProcessedBlocks", _log)))
                    .As<ILastTrackedBlockRepository>();

            builder.RegisterInstance(new FailedWebHookEventRepository(
                AzureTableStorage<FailedWebHookEventEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "FailedWebHookEvents", _log)))
                    .As<IFailedWebHookEventRepository>();
            
            builder.RegisterInstance(new PendingCashoutTransactionRepository(
                AzureTableStorage<TrackedCashoutTransactionEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "PengingCashoutTransactions", _log)))
                    .As<IPendingCashoutTransactionRepository>();

            builder.RegisterInstance(new CashOutOperationRepository(
                AzureTableStorage<CashOutOperationTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "CashOutOperations", _log)))
                    .As<ICashOutOperationRepository>();
        }

        private void RegisterQueue(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AzureQueueFactory(_settings.Nested(p => p.Db.DataConnString)))
                .As<IQueueFactory>();

            builder.RegisterGeneric(typeof(QueueRouter<>)).As(typeof(IQueueRouter<>)).InstancePerDependency();
        }
    }
}
