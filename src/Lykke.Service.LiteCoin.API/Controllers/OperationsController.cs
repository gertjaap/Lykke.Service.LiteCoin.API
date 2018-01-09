using System;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.LiteCoin.API.Core.Address;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.Operation;
using Lykke.Service.LiteCoin.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class OperationsController:Controller
    {
        private readonly IOperationService _operationService;
        private readonly IAddressValidator _addressValidator;

        public OperationsController(IOperationService operationService, 
            IAddressValidator addressValidator)
        {
            _operationService = operationService;
            _addressValidator = addressValidator;
        }

        [HttpPost("api/transactions")]
        [ProducesResponseType(typeof(BuildTransactionResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<BuildTransactionResponse> Build([FromBody] BuildTransactionRequest request)
        {
            if (request == null)
            {
                throw new BusinessException("Unable deserialize request", ErrorCode.BadInputParameter);
            }

            var amountSatoshi = MoneyConversionHelper.SatoshiFromContract(request.Amount);

            if (amountSatoshi <= 0)
            {
                throw new BusinessException($"Amount can't be less or equal to zero: {amountSatoshi}", ErrorCode.BadInputParameter);
            }

            if (request.AssetId != Constants.AssetsContants.LiteCoin.AssetId)
            {

                throw new BusinessException("Invalid assetId", ErrorCode.BadInputParameter);
            }

            var toBitcoinAddress = _addressValidator.GetBitcoinAddress(request.ToAddress);
            if (toBitcoinAddress == null)
            {

                throw new BusinessException("Invalid ToAddress ", ErrorCode.BadInputParameter);
            }

            var fromBitcoinAddress = _addressValidator.GetBitcoinAddress(request.FromAddress);
            if (fromBitcoinAddress == null)
            {

                throw new BusinessException("Invalid FromAddress", ErrorCode.BadInputParameter);
            }

            if (request.OperationId == Guid.Empty)
            {
                throw new BusinessException("Invalid operation id (GUID)", ErrorCode.BadInputParameter);
            }


            var tx = await _operationService.BuildTransferTransaction(request.OperationId, fromBitcoinAddress, toBitcoinAddress,
                request.AssetId, new Money(amountSatoshi), request.IncludeFee);



            return new BuildTransactionResponse
            {
                TransactionContext = tx.ToHex()
            };
        }
    }
}
