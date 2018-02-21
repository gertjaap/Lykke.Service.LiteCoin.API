using Autofac;
using Common.Log;
using Lykke.Service.Vertcoin.API.Core.Address;
using Lykke.Service.Vertcoin.API.Core.BlockChainReaders;
using Lykke.Service.Vertcoin.API.Core.Broadcast;
using Lykke.Service.Vertcoin.API.Core.Fee;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Lykke.Service.Vertcoin.API.Core.Operation;
using Lykke.Service.Vertcoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.Vertcoin.API.Core.TransactionOutputs;
using Lykke.Service.Vertcoin.API.Core.Transactions;
using Lykke.Service.Vertcoin.API.Core.Wallet;
using Lykke.Service.Vertcoin.API.Services.Address;
using Lykke.Service.Vertcoin.API.Services.BlockChainProviders.InsightApi;
using Lykke.Service.Vertcoin.API.Services.Broadcast;
using Lykke.Service.Vertcoin.API.Services.Fee;
using Lykke.Service.Vertcoin.API.Services.ObservableOperation;
using Lykke.Service.Vertcoin.API.Services.Operations;
using Lykke.Service.Vertcoin.API.Services.TransactionOutputs;
using Lykke.Service.Vertcoin.API.Services.Transactions;
using Lykke.Service.Vertcoin.API.Services.Wallet;
using Lykke.SettingsReader;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Services.Binder
{
    public  class ServiceModule:Module
    {
        private readonly ILog _log;
        private readonly IReloadingManager<VertcoinApiSettings> _settings;
        public ServiceModule(IReloadingManager<VertcoinApiSettings> settings, ILog log)
        {
            _log = log;
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterNetwork(builder);
            RegisterFeeServices(builder);
            RegisterAddressValidatorServices(builder);
            RegisterInsightApiBlockChainReaders(builder);
            RegisterDetectorServices(builder);
            RegisterTransactionOutputsServices(builder);
            RegisterTransactionBuilderServices(builder);
            RegisterBroadcastServices(builder);
            RegisterObservableServices(builder);
        }

        private void RegisterNetwork(ContainerBuilder builder)
        {
            NBitcoin.Vertcoin.Networks.EnsureRegistered();

            builder.RegisterInstance(Network.GetNetwork(_settings.CurrentValue.Network)).As<Network>();
        }

        private void RegisterFeeServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new FeeRateFacade(_settings.CurrentValue.FeePerByte))
                .As<IFeeRateFacade>();

            builder.Register(x =>
            {
                var resolver = x.Resolve<IComponentContext>();
                return new FeeService(resolver.Resolve<IFeeRateFacade>(), 
                    _settings.CurrentValue.MinFeeValue, 
                    _settings.CurrentValue.MaxFeeValue);
            }).As<IFeeService>();
        }

        private void RegisterAddressValidatorServices(ContainerBuilder builder)
        {
            builder.RegisterType<AddressValidator>().As<IAddressValidator>();
        }

        private void RegisterInsightApiBlockChainReaders(ContainerBuilder builder)
        {
            builder.RegisterInstance(new InsightApiSettings
            {
                Url = _settings.CurrentValue.InsightAPIUrl
            }).SingleInstance();

            builder.RegisterType<InsightApiBlockChainProvider>().As<IBlockChainProvider>();
        }
        

        private void RegisterDetectorServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(new OperationsConfirmationsSettings
            {
                MinConfirmationsToDetectOperation = _settings.CurrentValue.MinConfirmationsToDetectOperation
            });
            
        }
        
        private void RegisterTransactionOutputsServices(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionOutputsService>().As<ITransactionOutputsService>();
        }

        private void RegisterTransactionBuilderServices(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionBuilderService>().As<ITransactionBuilderService>();
            builder.RegisterType<OperationService>().As<IOperationService>();
        }

        private void RegisterBroadcastServices(ContainerBuilder builder)
        {
            builder.RegisterType<BroadcastService>().As<IBroadcastService>();
        }

        private void RegisterObservableServices(ContainerBuilder builder)
        {
            builder.RegisterType<ObservableOperationService>().As<IObservableOperationService>();
            builder.RegisterType<WalletBalanceService>().As<IWalletBalanceService>();
        }
    }
}
