using System;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    [Route("api/operations")]
    public class OperationsController:Controller
    {
        private readonly IOperationService _operationService;
        private readonly IAddressValidator _addressValidator;
        private readonly IWalletService _walletService;

        public OperationsController(IOperationService operationService, 
            IAddressValidator addressValidator,
            IWalletService walletService)
        {
            _operationService = operationService;
            _addressValidator = addressValidator;
            _walletService = walletService;
        }

        /// <summary>
        /// Creates cash out transaction, signs it, then broadcast
        /// </summary>
        /// <returns>internal operation id</returns>
        [HttpPost("api/wallets/{address}/cashout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CashOut(string address, [FromBody] CashoutFromWalletRequest request)
        {
            if (request == null)
            {
                throw new BusinessException("Unable deserialize request", ErrorCode.BadInputParameter);
            }
            if (!long.TryParse(request.Amount, out var amount))
            {
                throw new BusinessException("Invalid amount string", ErrorCode.BadInputParameter);
            }
            if (amount <= 0)
            {
                throw new BusinessException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);
            }

            if (request.AssetId != Constants.AssetsContants.LiteCoin)
            {

                throw new BusinessException($"Invalid assetId: availiable asset ids", ErrorCode.BadInputParameter);
            }

            if (!_addressValidator.IsValid(request.To))
            {

                throw new BusinessException($"Invalid DestAddress {request.To}", ErrorCode.BadInputParameter);
            }

            if (request.OperationId == Guid.Empty)
            {
                throw new BusinessException("Invalid operation id (GUID)", ErrorCode.BadInputParameter);
            }

            var sourceWallet = await _walletService.GetByPublicAddress(address);

            if (sourceWallet == null)
            {
                throw new BusinessException($"Source wallet {address} not found", ErrorCode.BadInputParameter);
            }

            if (!sourceWallet.IsClientWallet)
            {
                throw new BusinessException($"Source wallet {address} is not client wallet", ErrorCode.BadInputParameter);
            }
            
            await _operationService.ProceedCashOutOperation(request.OperationId, sourceWallet,
                _addressValidator.GetBitcoinAddress(request.To), amount);


            return Ok();
        }
    }
}
