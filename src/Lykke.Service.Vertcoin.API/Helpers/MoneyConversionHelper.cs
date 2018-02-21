using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.Vertcoin.API.Core.Constants;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Helpers
{
    public class MoneyConversionHelper
    {
        public static string SatoshiToContract(long satoshi)
        {
            return Conversions.CoinsToContract(new Money(satoshi).ToUnit(MoneyUnit.BTC), Constants.Assets.Vertcoin.Accuracy);
        }

        public static long SatoshiFromContract(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            var btc = Conversions.CoinsFromContract(input, Constants.Assets.Vertcoin.Accuracy);

            return new Money(btc, MoneyUnit.BTC).Satoshi;
        }
    }
}
