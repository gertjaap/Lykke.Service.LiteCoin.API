using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.LiteCoin.API.Core.Constants;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Helpers
{
    public class MoneyConversionHelper
    {
        public static string SatoshiToContract(long satoshi)
        {
            return Conversions.CoinsToContract(new Money(satoshi).ToUnit(MoneyUnit.BTC), Constants.Assets.LiteCoin.Accuracy);
        }

        public static long SatoshiFromContract(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            var btc = Conversions.CoinsFromContract(input, Constants.Assets.LiteCoin.Accuracy);

            return new Money(btc, MoneyUnit.BTC).Satoshi;
        }
    }
}
