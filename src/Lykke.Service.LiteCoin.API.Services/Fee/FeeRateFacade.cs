using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Fee;

namespace Lykke.Service.LiteCoin.API.Services.Fee
{
    internal class FeeRateFacade:IFeeRateFacade
    {
        private readonly FeeSettings _feeSettings;

        public FeeRateFacade(FeeSettings feeSettings)
        {
            _feeSettings = feeSettings;
        }

        public Task<int> GetFeePerKiloByte()
        {
            return Task.FromResult(_feeSettings.DefaultFeePerKyloByte);
        }
    }
}
