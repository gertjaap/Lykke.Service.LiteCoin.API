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
        public int FeePerByte { get; set; } = 100; // use value greater than default value 100 000 litoshi per kb = 100 litoshi per b. Ref https://bitcoin.stackexchange.com/questions/53821/where-can-i-find-the-current-fee-level-for-ltc

        [Optional]
        public decimal MinFeeValue { get; set; } = 0.1m;
        [Optional]
        public decimal MaxFeeValue { get; set; } = 1m;
        
        [Optional]
        public int MinCashInConfirmationsCount { get; set; } = 3;

        [Optional]
        public int MinCashOutConfirmationsCount { get; set; } = 3;

        [Optional]
        public int BroadcastedOutputsExpirationDays { get; set; } = 7;

        [Optional]
        public int SpentOutputsExpirationDays { get; set; } = 7;

        [Optional]
        public int CashInExpirationDays { get; set; } = 180;

        [Optional]
        public int CashOutExpirationDays { get; set; } = 180;
    }
}
