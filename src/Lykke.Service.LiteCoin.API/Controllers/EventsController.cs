using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.BlockchainSignService.Client.AutorestClient.Models;
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
        [SwaggerOperation(nameof(GetCompletedTransaction))]
        [ProducesResponseType(typeof(CompletedTransactionContract[]),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<CompletedTransactionContract>> GetCompletedTransaction([FromQuery]int skip, [FromQuery]int take)
        {
            return (await _observableOperationService.GetCompletedOperations(skip, take)).Select(p =>
                new CompletedTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    Hash = p.TxHash,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated
                });
        }



        [HttpGet("api/transactions/in-progress")]
        [SwaggerOperation(nameof(GetFailedTransaction))]
        [ProducesResponseType(typeof(FailedTransactionContract[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<FailedTransactionContract>> GetFailedTransaction([FromQuery]int skip, [FromQuery]int take)
        {
            return (await _observableOperationService.GetFailedOperations(skip, take)).Select(p =>
                new FailedTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated
                });
        }
        
        [HttpGet("api/transactions/in-progress")]
        [SwaggerOperation(nameof(GetFailedTransaction))]
        [ProducesResponseType(typeof(FailedTransactionContract[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IEnumerable<FailedTransactionContract>> RemoveF([FromQuery]int skip, [FromQuery]int take)
        {
            return (await _observableOperationService.GetFailedOperations(skip, take)).Select(p =>
                new FailedTransactionContract
                {
                    Amount = MoneyConversionHelper.SatoshiToContract(p.AmountSatoshi),
                    OperationId = p.OperationId,
                    AssetId = p.AssetId,
                    FromAddress = p.FromAddress,
                    ToAddress = p.ToAddress,
                    Timestamp = p.Updated
                });
        }



        [HttpGet("api/transactions/observation")]
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
