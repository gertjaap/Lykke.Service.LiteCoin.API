using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.BlockchainSignService.Client.AutorestClient.Models;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Swashbuckle.AspNetCore.SwaggerGen;
using ErrorResponse = Lykke.Common.Api.Contract.Responses.ErrorResponse;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class BroadcastController:Controller
    {
        private readonly IBroadcastService _broadcastService;

        public BroadcastController(IBroadcastService broadcastService)
        {
            _broadcastService = broadcastService;
        }

        [HttpPost("api/transactions/broadcast")]
        [SwaggerOperation(nameof(BroadcastTransaction))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> BroadcastTransaction(BroadcastTransactionRequest request)
        {
            try
            {
                await _broadcastService.BroadCastTransaction(request.OperationId, request.SignedTransaction);
            }
            catch (BusinessException e) when(e.Code == ErrorCode.TransactionAlreadyBroadcasted)
            {
                return new StatusCodeResult(409);
            }

            return Ok();
        }
    }
}
