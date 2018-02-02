using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.Fee
{
    public interface IDynamicFeeRate
    {
        int SatoshiPerKylobite { get; }
    }

    public class DynamicFeeRate : IDynamicFeeRate
    {
        public int SatoshiPerKylobite { get; set; }

        public static DynamicFeeRate Create(int satoshiPerKb)
        {
            return new DynamicFeeRate
            {
                SatoshiPerKylobite = satoshiPerKb
            };
        }
    }

    public interface IDynamicFeeRateRepository
    {
        Task InsertOrReplacet(IDynamicFeeRate feeRate);

        Task<IDynamicFeeRate> Get();
    }
}
