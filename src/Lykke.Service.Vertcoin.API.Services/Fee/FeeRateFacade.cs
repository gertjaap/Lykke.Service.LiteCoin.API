using System.Threading.Tasks;
using Lykke.Service.Vertcoin.API.Core.Fee;

namespace Lykke.Service.Vertcoin.API.Services.Fee
{
    internal class FeeRateFacade:IFeeRateFacade
    {
        private readonly int _feePerByte;

        public FeeRateFacade(int feePerByte)
        {
            _feePerByte = feePerByte;
        }

        public Task<int> GetFeePerByte()
        {
            return Task.FromResult(_feePerByte);
        }
    }
}
