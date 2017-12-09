using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiteCoin.API.Core.Settings.ServiceSettings
{
    public class LiteCoinAPISettings
    {
        public DbSettings Db { get; set; }
        public string Network { get; set; }
        public string InsightAPIUrl { get; set; }
        public string SignFacadeUrl { get; set; }

        public string EventsWebHook { get; set; }

        public string[] SourceWallets { get; set; }

        [Optional]
        public int FeePerByte { get; set; } = 100; // use default value 100 000 litoshi per kb = 100 litoshi per b. Ref https://bitcoin.stackexchange.com/questions/53821/where-can-i-find-the-current-fee-level-for-ltc
    }
}
