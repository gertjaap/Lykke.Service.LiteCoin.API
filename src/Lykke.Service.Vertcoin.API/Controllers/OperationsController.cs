using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.Vertcoin.API.Core.Address;
using Lykke.Service.Vertcoin.API.Core.Broadcast;
using Lykke.Service.Vertcoin.API.Core.Constants;
using Lykke.Service.Vertcoin.API.Core.Exceptions;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Lykke.Service.Vertcoin.API.Core.Operation;
using Lykke.Service.Vertcoin.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Vertcoin.API.Controllers
{
    public class OperationsController:Controller
    {
        private readonly IOperationService _operationService;
        private readonly IAddressValidator _addressValidator;
        private readonly IBroadcastService _broadcastService;
        private readonly IObservableOperationService _observableOperationService;


        public OperationsController(IOperationService operationService, 
            IAddressValidator addressValidator, 
            IBroadcastService broadcastService, 
            IObservableOperationService observableOperationService)
        {
            _operationService = operationService;
            _addressValidator = addressValidator;
            _broadcastService = broadcastService;
            _observableOperationService = observableOperationService;
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

            if (request.AssetId != Constants.Assets.Vertcoin.AssetId)
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


            var tx = await _operationService.GetOrBuildTransferTransaction(request.OperationId, fromBitcoinAddress, toBitcoinAddress,
                request.AssetId, new Money(amountSatoshi), request.IncludeFee);



            return new BuildTransactionResponse
            {
                TransactionContext = tx.ToHex()
            };
        }

        [HttpPost("api/transactions/broadcast")]
        [SwaggerOperation(nameof(BroadcastTransaction))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> BroadcastTransaction([FromBody] BroadcastTransactionRequest request)
        {
            if (request == null)
            {
                throw new BusinessException("Unable deserialize request", ErrorCode.BadInputParameter);
            }

            try
            {
                await _broadcastService.BroadCastTransaction(request.OperationId, request.SignedTransaction);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.TransactionAlreadyBroadcasted)
            {
                return new StatusCodeResult(409);
            }
            catch (BusinessException e) when (e.Code == ErrorCode.OperationNotFound)
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            return Ok();
        }

        [HttpGet("api/transactions/broadcast/{operationId}")]
        [SwaggerOperation(nameof(GetObservableOperation))]
        [ProducesResponseType(typeof(BroadcastedTransactionResponse),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetObservableOperation(Guid operationId)
        {
            var result = await _observableOperationService.GetById(operationId);

            if (result == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }

            BroadcastedTransactionState MapState(BroadcastStatus status)
            {
                switch (status)
                {
                    case BroadcastStatus.Completed:
                        return BroadcastedTransactionState.Completed;
                    case BroadcastStatus.Failed:
                        return BroadcastedTransactionState.Failed;
                    case BroadcastStatus.InProgress:
                        return BroadcastedTransactionState.InProgress;
                    default:
                        throw new InvalidCastException($"Unknown mapping from {status} ");
                }
            }

            return Ok(new BroadcastedTransactionResponse
            {
                Amount = MoneyConversionHelper.SatoshiToContract(result.AmountSatoshi),
                Fee = MoneyConversionHelper.SatoshiToContract(result.FeeSatoshi),
                OperationId = result.OperationId,
                Hash = result.TxHash,
                Timestamp = result.Updated,
                State = MapState(result.Status)
            });
        }

        [HttpDelete("api/transactions/broadcast/{operationId}")]
        [SwaggerOperation(nameof(RemoveObservableOperation))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> RemoveObservableOperation(Guid operationId)
        {
            await _observableOperationService.DeleteOperations(operationId);

            return Ok();
        }

    }
}
