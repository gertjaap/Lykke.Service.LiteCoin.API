using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Filters;
using Lykke.Service.LiteCoin.API.Models.Operations;
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
        [HttpPost("cashout")]
        [ProducesResponseType(typeof(CashOutResponce), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<CashOutResponce> CashOut([FromBody] CashOutRequest request)
        {
            if (request.Amount <= 0)
                throw new BackendException("Amount can't be less or equal to zero", ErrorCode.BadInputParameter);

            if (!_addressValidator.IsValid(request.DestAddress))
            {

                throw new BackendException($"Invalid DestAddress {request.DestAddress}", ErrorCode.BadInputParameter);
            }

            var sourceWallet = await _walletService.GetByWalletId(request.SourceWalletId);

            if (sourceWallet == null)
            {
                throw new BackendException($"WalletId {request.SourceWalletId} not found", ErrorCode.BadInputParameter);
            }

            var operationId = Guid.NewGuid().ToString();

            await _operationService.ProceedCashOutOperation(operationId, sourceWallet,
                _addressValidator.GetBitcoinAddress(request.DestAddress), request.Amount);


            return new CashOutResponce
            {
                OperationId = operationId
            };
        }
    }
}
