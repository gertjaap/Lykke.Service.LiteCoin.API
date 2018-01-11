using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.LiteCoin.API.Core.ObservableOperation;
using Lykke.Service.LiteCoin.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class EventsController:Controller
    {
        private readonly IObservableOperationService _observableOperationService;

        public EventsController(IObservableOperationService observableOperationService)
        {
            _observableOperationService = observableOperationService;
        }


        [HttpGet("api/transactions/completed")]
        [SwaggerOperation(nameof(GetCompletedTransactions))]
        [ProducesResponseType(typeof(CompletedTransactionContract[]),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<CompletedTransactionContract>> GetCompletedTransactions()
        {
            return (await _observableOperationService.GetCompletedOperations()).Select(p =>
                new CompletedTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    Hash = p.TxHash,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated,
                    Fee = MoneyConversionHelper.SatoshiToContract(p.FeeSatoshi)
                });
        }



        [HttpGet("api/transactions/failed")]
        [SwaggerOperation(nameof(GetFailedTransactions))]
        [ProducesResponseType(typeof(FailedTransactionContract[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<FailedTransactionContract>> GetFailedTransactions()
        {
            return (await _observableOperationService.GetFailedOperations()).Select(p =>
                new FailedTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated,
                    Fee = MoneyConversionHelper.SatoshiToContract(p.FeeSatoshi)
                });
        }
        
        [HttpGet("api/transactions/in-progress")]
        [SwaggerOperation(nameof(GetInProgressTransactions))]
        [ProducesResponseType(typeof(InProgressTransactionContract[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<InProgressTransactionContract>> GetInProgressTransactions()
        {
            return (await _observableOperationService.GetInProgressOperations()).Select(p =>
                new InProgressTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated,
                    Fee = MoneyConversionHelper.SatoshiToContract(p.FeeSatoshi)
                });
        }



        [HttpDelete("api/transactions/observation")]
        [SwaggerOperation(nameof(DeleteTransactions))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> DeleteTransactions([FromBody] Guid[] operationIds)
        {
            await _observableOperationService.DeleteOperations(operationIds);

            return Ok();
        }

    }
}
