using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class FeeFacade:IFeeFacade
    {
        private readonly IFeeRateFacade _feeRateFacade;
        private readonly decimal _minFeeValue;
        private readonly decimal _maxFeeValue;

        public FeeFacade(IFeeRateFacade feeRateFacade, decimal minFeeValue, decimal maxFeeValue)
        {
            _feeRateFacade = feeRateFacade;
            _minFeeValue = minFeeValue;
            _maxFeeValue = maxFeeValue;
        }

        public async Task<Money> CalcFeeForTransaction(Transaction tx)
        {
            var size = tx.ToBytes().Length;

            return await CalcFee(size);
        }

        public async Task<Money> CalcFeeForTransaction(TransactionBuilder builder)
        {
            return builder.EstimateFees(builder.BuildTransaction(false), await GetFeeRate());
        }

        public async Task<FeeRate> GetFeeRate()
        {
            var feePerByte = await _feeRateFacade.GetFeePerByte();

            return new FeeRate(new Money(feePerByte * 1024, MoneyUnit.Satoshi));
        }

        private async Task<Money> CalcFee(int size)
        {
            var  fromFeeRate = (await GetFeeRate()).GetFee(size);

            if (fromFeeRate.Satoshi > _maxFeeValue)
            {
                return new Money(_maxFeeValue, MoneyUnit.Satoshi);
            }

            if (fromFeeRate.Satoshi < _minFeeValue)
            {
                return new Money(_minFeeValue, MoneyUnit.Satoshi);
            }

            return fromFeeRate;
        }
    }
}
