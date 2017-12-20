using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.WebHook;

namespace Lykke.Job.LiteCoin.Functions
{
    public class RetryFailedEventsFunctions
    {
        private readonly IFailedWebHookEventRepository _failedWebHookEventRepository;
        private readonly IQueueRouter<CashInNotificationContext> _cashInQueue;
        private readonly IQueueRouter<CashOutStartedNotificationContext> _cashOutStartedQueue;
        private readonly IQueueRouter<CashOutCompletedNotificationContext> _cashOutCompletedQueue;

        public RetryFailedEventsFunctions(IFailedWebHookEventRepository failedWebHookEventRepository, 
            IQueueRouter<CashInNotificationContext> cashInQueue, 
            IQueueRouter<CashOutStartedNotificationContext> cashOutStartedQueue, 
            IQueueRouter<CashOutCompletedNotificationContext> cashOutCompletedQueue)
        {
            _failedWebHookEventRepository = failedWebHookEventRepository;
            _cashInQueue = cashInQueue;
            _cashOutStartedQueue = cashOutStartedQueue;
            _cashOutCompletedQueue = cashOutCompletedQueue;
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
