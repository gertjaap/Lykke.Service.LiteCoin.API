using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Filters;
using Lykke.Service.LiteCoin.API.Models;
using Lykke.Service.LiteCoin.API.Models.Wallet;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    [Route("api/wallet")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        [HttpPost]
        [SwaggerOperation("CreateWallet")]
        [ProducesResponseType(typeof(WalletCreationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> CreateWallet()
        {
            var result = await _walletService.CreateWallet();

            return Ok(new WalletCreationResponse
            {
                PublicAddress = result.Address.ToString(),
                WalletId = result.WalletId
            });
        }
    }
}
