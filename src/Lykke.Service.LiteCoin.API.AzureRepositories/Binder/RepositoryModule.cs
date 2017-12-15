using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.LiteCoin.API.AzureRepositories.CashOut;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations;
using Lykke.Service.LiteCoin.API.AzureRepositories.Operations.CashIn;
using Lykke.Service.LiteCoin.API.AzureRepositories.Queue;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.TransactionOutput.SpentOutputs;
using Lykke.Service.LiteCoin.API.AzureRepositories.TxTracker;
using Lykke.Service.LiteCoin.API.AzureRepositories.WebHook;
using Lykke.Service.LiteCoin.API.Core.BlockChainTracker;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
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
            builder.RegisterInstance(new CashInLastProcessedBlockRepository(
                AzureTableStorage<LastCashInProcessedBlockEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                    "CashInsLastProcessedBlocks", _log)))
                    .As<ICashInLastProcessedBlockRepository>();

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


            builder.RegisterInstance(new CashInOperationRepository(
                    AzureTableStorage<CashInOperationEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "CashInOperations", _log)))
                .As<ICashInOperationRepository>();

            builder.RegisterInstance(new BroadcastedOutputRepository(
                    AzureTableStorage<BroadcastedOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "BroadcastedOutputs", _log)))
                .As<IBroadcastedOutputRepository>();

            builder.RegisterInstance(new SpentOutputRepository(
                    AzureTableStorage<SpentOutputTableEntity>.Create(_settings.Nested(p => p.Db.DataConnString),
                        "SpentOutputs", _log)))
                .As<ISpentOutputRepository>();
        }

        private void RegisterQueue(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AzureQueueFactory(_settings.Nested(p => p.Db.DataConnString)))
                .As<IQueueFactory>();

            builder.RegisterGeneric(typeof(QueueRouter<>)).As(typeof(IQueueRouter<>)).InstancePerDependency();
        }
    }
}
