using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.Vertcoin.API.Core.Services;
using Lykke.Service.Vertcoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.Vertcoin.API.Services;
using Lykke.Service.Vertcoin.API.Services.Health;
using Lykke.Service.Vertcoin.API.Services.LifeiteManagers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.Vertcoin.API.Modules
{
    public class VertcoinApiModule : Module
    {
        private readonly IReloadingManager<VertcoinApiSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public VertcoinApiModule(IReloadingManager<VertcoinApiSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.Populate(_services);
        }
    }
}
