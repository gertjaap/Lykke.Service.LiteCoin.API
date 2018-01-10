using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.LiteCoin.API.Core.Constants;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Helpers
{
    public class MoneyConversionHelper
    {
        public static string SatoshiToContract(long satoshi)
        {
            return Conversions.CoinsToContract(new Money(satoshi).ToUnit(MoneyUnit.Satoshi), Constants.AssetsContants.LiteCoin.Accuracy);
        }

        public static long SatoshiFromContract(string input)
        {
            var btc = Conversions.CoinsFromContract(input, Constants.AssetsContants.LiteCoin.Accuracy);

            return new Money(btc, MoneyUnit.BTC).Satoshi;
        }
    }
}
