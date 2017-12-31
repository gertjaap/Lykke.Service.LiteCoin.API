using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Responses;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        [HttpPost("api/wallets")]
        [SwaggerOperation("CreateWallet")]
        [ProducesResponseType(typeof(WalletCreationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponce), 400)]
        public async Task<IActionResult> CreateWallet()
        {
            var result = await _walletService.CreateWallet();

            return Ok(new WalletCreationResponse
            {
                Address = result.Address.ToString(),
            });
        }
    }
}
