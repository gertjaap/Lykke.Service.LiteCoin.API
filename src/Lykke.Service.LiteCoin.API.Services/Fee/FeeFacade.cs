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

        public FeeFacade(IFeeRateFacade feeRateFacade)
        {
            _feeRateFacade = feeRateFacade;
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

            return new FeeRate(new Money(feePerByte * 1000, MoneyUnit.Satoshi));
        }

        public async Task<Money> CalcFee(int size)
        {
            return (await GetFeeRate()).GetFee(size);
        }
    }
}
