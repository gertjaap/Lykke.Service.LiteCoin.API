using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    public class FeeRateFacade:IFeeRateFacade
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
