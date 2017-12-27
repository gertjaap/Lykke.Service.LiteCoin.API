using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.WebHook;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendWebHookFunctions
    {
        private readonly IWebHookSender _webHookSender;
        private readonly ICashInOperationRepository _cashInOperationRepository;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly ICashOutEventRepository _cashOutEventRepository;
        private readonly ICashInEventRepository _cashInEventRepository;
        private readonly ILog _log;


        private readonly IFailedWebHookEventRepository _failedWebHookEventRepository;
        private readonly IQueueRouter<CashInNotificationContext> _cashInQueue;
        private readonly IQueueRouter<CashOutStartedNotificationContext> _cashOutStartedQueue;
        private readonly IQueueRouter<CashOutCompletedNotificationContext> _cashOutCompletedQueue;

        public SendWebHookFunctions(IWebHookSender webHookSender, 
            ICashInOperationRepository cashInOperationRepository,
            ICashOutOperationRepository cashOutOperationRepository, 
            ICashOutEventRepository cashOutEventRepository, 
            ICashInEventRepository cashInEventRepository,
            ILog log,
            IFailedWebHookEventRepository failedWebHookEventRepository, 
            IQueueRouter<CashInNotificationContext> cashInQueue,
            IQueueRouter<CashOutStartedNotificationContext> cashOutStartedQueue, 
            IQueueRouter<CashOutCompletedNotificationContext> cashOutCompletedQueue)
        {
            _webHookSender = webHookSender;
            _cashInOperationRepository = cashInOperationRepository;
            _cashOutOperationRepository = cashOutOperationRepository;
            _cashOutEventRepository = cashOutEventRepository;
            _cashInEventRepository = cashInEventRepository;
            _log = log;
            _failedWebHookEventRepository = failedWebHookEventRepository;
            _cashInQueue = cashInQueue;
            _cashOutStartedQueue = cashOutStartedQueue;
            _cashOutCompletedQueue = cashOutCompletedQueue;
        }

        [QueueTrigger(CashInNotificationContext.QueueName, notify:true)]
        public async Task SendCashInNotification(CashInNotificationContext context)
        {
            var operation = await _cashInOperationRepository.GetByOperationId(context.OperationId);

            if (operation == null)
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashInNotification),
                    context.ToJson(), "Operation not found");

                return;
            }

            var eventType = CashInEventType.NotificationSend;

            if (!await _cashInEventRepository.Exist(operation.OperationId, eventType))
            {
                await _webHookSender.ProcessCashIn(operationId: context.OperationId, dateTime: operation.DetectedAt,
                    walletId: operation.DestinationAddress, assetId: operation.AssetId, amount: operation.Amount,
                    sourceAddress: operation.SourceAddress);

                await _cashInEventRepository.InsertEvent(CashInEvent.Create(operation.OperationId, eventType));
            }
            else
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashInNotification),
                    context.ToJson(), "Attempt to double send webhook");
            }
        }


        [QueueTrigger(CashOutStartedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutStartedNotification(CashOutStartedNotificationContext context)
        {
            var operation = await _cashOutOperationRepository.GetByOperationId(context.OperationId);

            if (operation == null)
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashOutStartedNotification),
                    context.ToJson(), "Operation not found");

                return;
            }

            var eventType = CashOutEventType.NotificationOnStartSend;

            if (!await _cashOutEventRepository.Exist(operation.OperationId, eventType))
            {
                await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: operation.StartedAt,
                    walletId: operation.ClientWalletId, assetId: operation.AssetId, amount: operation.Amount,
                    destAddress: operation.DestinationAddress, txHash: operation.TxHash);

                await _cashOutEventRepository.InsertEvent(CashOutEvent.Create(operation.OperationId, eventType));
            }
            else
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashOutStartedNotification),
                    context.ToJson(), "Attempt to double send webhook");
            }
        }

        [QueueTrigger(CashOutCompletedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutCompletedNotification(CashOutCompletedNotificationContext context)
        {
            var operation = await _cashOutOperationRepository.GetByOperationId(context.OperationId);

            if (operation == null)
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashOutCompletedNotification),
                    context.ToJson(), "Operation not found");

                return;
            }

            var eventType = CashOutEventType.NotificationOnCompletedSend;

            if (!await _cashOutEventRepository.Exist(operation.OperationId, eventType))
            {

                await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: operation.StartedAt,
                    walletId: operation.ClientWalletId, assetId: operation.AssetId, amount: operation.Amount,
                    destAddress: operation.DestinationAddress, txHash: operation.TxHash);

                await _cashOutEventRepository.InsertEvent(CashOutEvent.Create(operation.OperationId, eventType));
            }
            else
            {
                await _log.WriteWarningAsync(nameof(SendWebHookFunctions), nameof(SendCashOutCompletedNotification),
                    context.ToJson(), "Attempt to double send webhook");
            }
        }

        [TimerTrigger("01:00:00")]
        public async Task Execute()
        {
            var failedEvents = await _failedWebHookEventRepository.GetAll();
            foreach (var failedWebHookEvent in failedEvents)
            {
                switch (failedWebHookEvent.WebHookType)
                {
                    case WebHookType.CashIn:
                        await _cashInQueue.AddMessage(new CashInNotificationContext
                        {
                            OperationId = failedWebHookEvent.OperationId
                        });
                        break;
                    case WebHookType.CashOutStarted:
                        await _cashOutStartedQueue.AddMessage(new CashOutStartedNotificationContext
                        {
                            OperationId = failedWebHookEvent.OperationId
                        });
                        break;
                    case WebHookType.CashOutCompleted:
                        await _cashOutCompletedQueue.AddMessage(new CashOutCompletedNotificationContext
                        {
                            OperationId = failedWebHookEvent.OperationId
                        });
                        break;
                    default:
                        throw new InvalidCastException($"Unexpected {nameof(failedWebHookEvent.WebHookType)}: {failedWebHookEvent.WebHookType}");
                }
            }
        }
    }
}
