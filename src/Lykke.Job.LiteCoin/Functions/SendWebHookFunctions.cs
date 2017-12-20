using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.WebHook;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendWebHookFunctions
    {
        private readonly IWebHookSender _webHookSender;
        private readonly ICashInOperationRepository _cashInOperationRepository;
        private readonly ICashOutOperationRepository _cashOutOperationRepository;

        public SendWebHookFunctions(IWebHookSender webHookSender, 
            ICashInOperationRepository cashInOperationRepository,
            ICashOutOperationRepository cashOutOperationRepository)
        {
            _webHookSender = webHookSender;
            _cashInOperationRepository = cashInOperationRepository;
            _cashOutOperationRepository = cashOutOperationRepository;
        }

        [QueueTrigger(CashInNotificationContext.QueueName, notify:true)]
        public async Task SendCashInNotification(CashInNotificationContext context)
        {
            var operation = await _cashInOperationRepository.GetByOperationId(context.OperationId);

            await _webHookSender.ProcessCashIn(operationId: context.OperationId, dateTime: operation.DetectedAt,
                walletId: operation.DestinationWalletId, assetId: operation.AssetId, amount: operation.Amount,
                sourceAddress: operation.SourceAddress);
        }


        [QueueTrigger(CashOutStartedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutStartedNotification(CashOutStartedNotificationContext context)
        {
            var operation = await _cashOutOperationRepository.GetByOperationId(context.OperationId);

            await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: operation.StartedAt,
                walletId: operation.ClientWalletId, assetId: operation.AssetId, amount: operation.Amount,
                destAddress: operation.DestinationAddress, txHash: operation.TxHash);
        }

        [QueueTrigger(CashOutCompletedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutCompletedNotification(CashOutCompletedNotificationContext context)
        {
            var operation = await _cashOutOperationRepository.GetByOperationId(context.OperationId);

            await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: operation.StartedAt,
                walletId: operation.ClientWalletId, assetId: operation.AssetId, amount: operation.Amount,
                destAddress: operation.DestinationAddress, txHash: operation.TxHash);
        }
    }
}
