using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Requests;
using Lykke.Service.BlockchainApi.Contract.Responses.PendingEvents;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.LiteCoin.API.Controllers
{
    public class PendingEventsController:Controller
    {
        private readonly IPendingCashOutEventRepository _cashOutEventRepository;
        private readonly IPendingCashInEventRepository _cashInEventRepository;

        public PendingEventsController(IPendingCashOutEventRepository cashOutEventRepository, IPendingCashInEventRepository cashInEventRepository)
        {
            _cashOutEventRepository = cashOutEventRepository;
            _cashInEventRepository = cashInEventRepository;
        }

        [HttpGet("/api/pending-events/cashin")]
        [SwaggerOperation(nameof(GetPendingCashInEvents))]
        [ProducesResponseType(typeof(IEnumerable<PendingCashinEventContract>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IEnumerable<PendingCashinEventContract>> GetPendingCashInEvents([FromQuery] int maxEventsNumber = 100)
        {
            var entities = await _cashInEventRepository.GetAll(PendingCashInEventStatusType.DetectedOnBlockChain, maxEventsNumber);

            return entities.Select(p => new PendingCashinEventContract
            {
                OperationId = p.OperationId.ToString(),
                AssetId = p.AssetId,
                Amount = p.Amount.ToString("F"),
                Timestamp = p.DetectedAt
            });
        }

        [HttpGet("/api/pending-events/cashout-started")]
        [SwaggerOperation(nameof(GetPendingCashOutStartedEvents))]
        [ProducesResponseType(typeof(IEnumerable<PendingCashoutStartedEventContract>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IEnumerable<PendingCashoutStartedEventContract>> GetPendingCashOutStartedEvents([FromQuery] int maxEventsNumber = 100)
        {
            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Started, maxEventsNumber);

            return entities.Select(p => new PendingCashoutStartedEventContract
            {
                OperationId = p.OperationId.ToString(),
                AssetId = p.AssetId,
                Amount = p.Amount.ToString("F"),
                Timestamp = p.StartedAt,
                TransactionHash = p.TxHash,
                ToAddress = p.DestinationAddress
            });
        }



        [HttpGet("/api/pending-events/cashout-completed")]
        [SwaggerOperation(nameof(GetPendingCashOutCompletedEvents))]
        [ProducesResponseType(typeof(IEnumerable<PendingCashoutCompletedEventContract>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IEnumerable<PendingCashoutCompletedEventContract>> GetPendingCashOutCompletedEvents([FromQuery] int maxEventsNumber = 100)
        {
            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Completed, maxEventsNumber);

            return entities.Select(p => new PendingCashoutCompletedEventContract
            {
                OperationId = p.OperationId.ToString(),
                AssetId = p.AssetId,
                Amount = p.Amount.ToString("F"),
                Timestamp = p.StartedAt,
                TransactionHash = p.TxHash,
                ToAddress = p.DestinationAddress
            });
        }

        [HttpGet("/api/pending-events/cashout-failed")]
        [SwaggerOperation(nameof(GetPendingCashOutFailedEvents))]
        [ProducesResponseType(typeof(IEnumerable<PendingCashoutFailedEventContract>), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IEnumerable<PendingCashoutFailedEventContract>> GetPendingCashOutFailedEvents([FromQuery] int maxEventsNumber = 100)
        {
            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Failed, maxEventsNumber);

            return entities.Select(p => new PendingCashoutFailedEventContract
            {
                OperationId = p.OperationId.ToString(),
                AssetId = p.AssetId,
                Amount = p.Amount.ToString("F"),
                Timestamp = p.StartedAt,
                ToAddress = p.DestinationAddress
            });
        }

        [HttpDelete("/api/pending-events/cashin")]
        [SwaggerOperation(nameof(DeletePendingCashInEvents))]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> DeletePendingCashInEvents([FromBody]RemovePendingEventsRequest request)
        {
            if (request.OperationIds.Any(x => !Guid.TryParse(x, out _)))
            {
               throw new BusinessException("Invalid operation id (not guid)", ErrorCode.BadInputParameter);
            }

            await _cashInEventRepository.DeleteBatchIfExist(PendingCashInEventStatusType.DetectedOnBlockChain, request.OperationIds.Select(Guid.Parse));

            return Ok();
        }

        [HttpDelete("/api/pending-events/cashout-started")]
        [SwaggerOperation(nameof(DeletePendingCashOutStartedEvents))]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> DeletePendingCashOutStartedEvents([FromBody]RemovePendingEventsRequest request)
        {
            if (request.OperationIds.Any(x => !Guid.TryParse(x, out _)))
            {
                throw new BusinessException("Invalid operation id (not guid)", ErrorCode.BadInputParameter);
            }

            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Started, request.OperationIds.Select(Guid.Parse));

            return Ok();
        }

        [HttpDelete("/api/pending-events/cashout-completed")]
        [SwaggerOperation(nameof(DeletePendingCashOutCompletedEvents))]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> DeletePendingCashOutCompletedEvents([FromBody]RemovePendingEventsRequest request)
        {
            if (request.OperationIds.Any(x => !Guid.TryParse(x, out _)))
            {
                throw new BusinessException("Invalid operation id (not guid)", ErrorCode.BadInputParameter);
            }

            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Completed, request.OperationIds.Select(Guid.Parse));

            return Ok();
        }

        [HttpDelete("/api/pending-events/cashout-failed")]
        [SwaggerOperation(nameof(DeletePendingCashOutFailedEvents))]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        public async Task<IActionResult> DeletePendingCashOutFailedEvents([FromBody]RemovePendingEventsRequest request)
        {
            if (request.OperationIds.Any(x => !Guid.TryParse(x, out _)))
            {
                throw new BusinessException("Invalid operation id (not guid)", ErrorCode.BadInputParameter);
            }

            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Failed, request.OperationIds.Select(Guid.Parse));

            return Ok();
        }
    }
}
