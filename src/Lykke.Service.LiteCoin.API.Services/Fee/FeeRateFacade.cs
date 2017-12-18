using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    internal class FeeRateFacade:IFeeRateFacade
    {
        private readonly int _feePerByte;
        private readonly int _feeRateMultiplayer;

        public FeeRateFacade(int feePerByte, int feeRateMultiplayer)
        {
            _feePerByte = feePerByte;
            _feeRateMultiplayer = feeRateMultiplayer;
        }

        public Task<int> GetFeePerByte()
        {
            return Task.FromResult(_feePerByte * _feeRateMultiplayer);
        }
    }
}
