using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainSignService.Client.AutorestClient.Models;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using ErrorResponse = Lykke.Common.Api.Contract.Responses.ErrorResponse;

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
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CreateWallet()
        {
            var result = await _walletService.CreateWallet();

            return Ok(new WalletCreationResponse
            {
                PublicAddress = result.Address.ToString()
            });
        }
    }
}
