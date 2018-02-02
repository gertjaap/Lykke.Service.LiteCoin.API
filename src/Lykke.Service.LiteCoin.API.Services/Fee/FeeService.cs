using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class FeeService:IFeeService
    {
        private readonly IFeeRateFacade _feeRateFacade;
        private readonly FeeSettings _feeSettings;

        public FeeService(IFeeRateFacade feeRateFacade, FeeSettings feeSettings)
        {
            _feeRateFacade = feeRateFacade;
            _feeSettings = feeSettings;
        }

        public async Task<Money> CalcFeeForTransaction(Transaction tx)
        {
            var size = tx.ToBytes().Length;

            return await CalcFee(size);
        }

        public  Task<Money> CalcFeeForTransaction(TransactionBuilder builder)
        {
            return CalcFeeForTransaction(builder.BuildTransaction(false));
        }

        public async Task<FeeRate> GetFeeRate()
        {
            var feePerByte = await _feeRateFacade.GetFeePerKiloByte();

            return new FeeRate(new Money(feePerByte, MoneyUnit.Satoshi));
        }

        private async Task<Money> CalcFee(int size)
        {
            var  fromFeeRate = (await GetFeeRate()).GetFee(size);

            var min = new Money(_feeSettings.MinFeeValueSatoshi, MoneyUnit.Satoshi);
            var max = new Money(_feeSettings.MaxFeeValueSatoshi, MoneyUnit.Satoshi);

            return Money.Max(Money.Min(fromFeeRate, max), min);
        }
    }
}
