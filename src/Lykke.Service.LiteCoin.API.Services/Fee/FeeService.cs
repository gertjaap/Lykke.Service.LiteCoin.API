using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class FeeService:IFeeService
    {
        private readonly IFeeRateFacade _feeRateFacade;
        private readonly decimal _minFeeValueSatoshi;
        private readonly decimal _maxFeeValueSatoshi;

        public FeeService(IFeeRateFacade feeRateFacade, decimal minFeeValueSatoshi, decimal maxFeeValueSatoshi)
        {
            _feeRateFacade = feeRateFacade;
            _minFeeValueSatoshi = minFeeValueSatoshi;
            _maxFeeValueSatoshi = maxFeeValueSatoshi;
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
            var feePerByte = await _feeRateFacade.GetFeePerByte();

            return new FeeRate(new Money(feePerByte * 1024, MoneyUnit.Satoshi));
        }

        private async Task<Money> CalcFee(int size)
        {
            var  fromFeeRate = (await GetFeeRate()).GetFee(size);

            if (fromFeeRate.Satoshi > _maxFeeValueSatoshi)
            {
                return new Money(_maxFeeValueSatoshi, MoneyUnit.Satoshi);
            }

            if (fromFeeRate.Satoshi < _minFeeValueSatoshi)
            {
                return new Money(_minFeeValueSatoshi, MoneyUnit.Satoshi);
            }

            return fromFeeRate;
        }
    }
}
