using Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings;
using Lykke.Service.LiteCoin.API.Core.Settings.SlackNotifications;

namespace Lykke.Service.LiteCoin.API.Core.Settings
{
    public class AppSettings
    {
        public LiteCoinAPISettings LiteCoinAPI { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
