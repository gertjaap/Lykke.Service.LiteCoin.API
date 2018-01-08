//using System.Linq;
//using System.Threading.Tasks;
//using Lykke.Service.BlockchainApi.Contract.Requests;
//using Lykke.Service.BlockchainApi.Contract.Responses;
//using Lykke.Service.BlockchainApi.Contract.Responses.PendingEvents;
//using Lykke.Service.LiteCoin.API.Core.CashIn;
//using Lykke.Service.LiteCoin.API.Core.CashOut;
//using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using ErrorResponse = Lykke.Common.Api.Contract.Responses.ErrorResponse;

//namespace Lykke.Service.LiteCoin.API.Controllers
//{
//    public class PendingEventsController:Controller
//    {
//        private readonly IPendingCashOutEventRepository _cashOutEventRepository;
//        private readonly IPendingCashInEventRepository _cashInEventRepository;

//        public PendingEventsController(IPendingCashOutEventRepository cashOutEventRepository, IPendingCashInEventRepository cashInEventRepository)
//        {
//            _cashOutEventRepository = cashOutEventRepository;
//            _cashInEventRepository = cashInEventRepository;
//        }

//        [HttpGet("/api/pending-events/cashin")]
//        [SwaggerOperation(nameof(GetPendingCashInEvents))]
//        [ProducesResponseType(typeof(PendingEventsResponse<PendingCashinEventContract>), 200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<PendingEventsResponse<PendingCashinEventContract>> GetPendingCashInEvents([FromQuery] int maxEventsNumber = 100)
//        {
//            var entities = await _cashInEventRepository.GetAll(PendingCashInEventStatusType.DetectedOnBlockChain, maxEventsNumber);

//            return new PendingEventsResponse<PendingCashinEventContract>
//            {
//                Events = entities.Select(p => new PendingCashinEventContract
//                {
//                    OperationId = p.OperationId,
//                    AssetId = p.AssetId,
//                    Amount = p.Amount.ToString("D"),
//                    Timestamp = p.DetectedAt,
//                    Address = p.DestinationAddress
//                }).ToList().AsReadOnly()
//            };
//        }

//        [HttpGet("/api/pending-events/cashout-started")]
//        [SwaggerOperation(nameof(GetPendingCashOutStartedEvents))]
//        [ProducesResponseType(typeof(PendingEventsResponse<PendingCashoutStartedEventContract>), 200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<PendingEventsResponse<PendingCashoutStartedEventContract>> GetPendingCashOutStartedEvents([FromQuery] int maxEventsNumber = 100)
//        {
//            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Started, maxEventsNumber);

//            return new PendingEventsResponse<PendingCashoutStartedEventContract>
//            {
//                Events = entities.Select(p => new PendingCashoutStartedEventContract
//                {
//                    OperationId = p.OperationId,
//                    AssetId = p.AssetId,
//                    Amount = p.Amount.ToString("D"),
//                    Timestamp = p.StartedAt,
//                    TransactionHash = p.TxHash,
//                    ToAddress = p.DestinationAddress,
//                    FromAddress = p.ClientAddress
//                }).ToList().AsReadOnly()
//            };
//        }

//        [HttpGet("/api/pending-events/cashout-completed")]
//        [SwaggerOperation(nameof(GetPendingCashOutCompletedEvents))]
//        [ProducesResponseType(typeof(PendingEventsResponse<PendingCashoutCompletedEventContract>), 200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<PendingEventsResponse<PendingCashoutCompletedEventContract>> GetPendingCashOutCompletedEvents([FromQuery] int maxEventsNumber = 100)
//        {
//            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Completed, maxEventsNumber);

//            return new PendingEventsResponse<PendingCashoutCompletedEventContract>
//            {
//                Events = entities.Select(p => new PendingCashoutCompletedEventContract
//                {
//                    OperationId = p.OperationId,
//                    AssetId = p.AssetId,
//                    Amount = p.Amount.ToString("D"),
//                    Timestamp = p.StartedAt,
//                    TransactionHash = p.TxHash,
//                    ToAddress = p.DestinationAddress,
//                    FromAddress = p.ClientAddress
//                }).ToList().AsReadOnly()
//            };
//        }

//        [HttpGet("/api/pending-events/cashout-failed")]
//        [SwaggerOperation(nameof(GetPendingCashOutFailedEvents))]
//        [ProducesResponseType(typeof(PendingEventsResponse<PendingCashoutFailedEventContract>), 200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<PendingEventsResponse<PendingCashoutFailedEventContract>> GetPendingCashOutFailedEvents([FromQuery] int maxEventsNumber = 100)
//        {
//            var entities = await _cashOutEventRepository.GetAll(PendingCashOutEventStatusType.Failed, maxEventsNumber);

//            return new PendingEventsResponse<PendingCashoutFailedEventContract>
//            {
//                Events = entities.Select(p => new PendingCashoutFailedEventContract
//                {
//                    OperationId = p.OperationId,
//                    AssetId = p.AssetId,
//                    Amount = p.Amount.ToString("D"),
//                    Timestamp = p.StartedAt,
//                    ToAddress = p.DestinationAddress,
//                    FromAddress = p.ClientAddress
//                }).ToList().AsReadOnly()
//            };
//        }

//        [HttpDelete("/api/pending-events/cashin")]
//        [SwaggerOperation(nameof(DeletePendingCashInEvents))]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<IActionResult> DeletePendingCashInEvents([FromBody]RemovePendingEventsRequest request)
//        {
//            await _cashInEventRepository.DeleteBatchIfExist(PendingCashInEventStatusType.DetectedOnBlockChain, request.OperationIds);

//            return Ok();
//        }

//        [HttpDelete("/api/pending-events/cashout-started")]
//        [SwaggerOperation(nameof(DeletePendingCashOutStartedEvents))]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<IActionResult> DeletePendingCashOutStartedEvents([FromBody]RemovePendingEventsRequest request)
//        {
//            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Started, request.OperationIds);

//            return Ok();
//        }

//        [HttpDelete("/api/pending-events/cashout-completed")]
//        [SwaggerOperation(nameof(DeletePendingCashOutCompletedEvents))]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<IActionResult> DeletePendingCashOutCompletedEvents([FromBody]RemovePendingEventsRequest request)
//        {
//            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Completed, request.OperationIds);

//            return Ok();
//        }

//        [HttpDelete("/api/pending-events/cashout-failed")]
//        [SwaggerOperation(nameof(DeletePendingCashOutFailedEvents))]
//        [ProducesResponseType(200)]
//        [ProducesResponseType(typeof(ErrorResponse), 400)]
//        public async Task<IActionResult> DeletePendingCashOutFailedEvents([FromBody]RemovePendingEventsRequest request)
//        {
//            await _cashOutEventRepository.DeleteBatchIfExist(PendingCashOutEventStatusType.Failed, request.OperationIds);

//            return Ok();
//        }
//    }
//}
