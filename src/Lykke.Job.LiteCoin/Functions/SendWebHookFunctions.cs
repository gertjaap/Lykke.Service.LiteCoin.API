using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.WebHook;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendWebHookFunctions
    {
        private readonly IWebHookSender _webHookSender;

        public SendWebHookFunctions(IWebHookSender webHookSender)
        {
            _webHookSender = webHookSender;
        }

        [QueueTrigger(CashInNotificationContext.QueueName, notify:true)]
        public async Task SendCashInNotification(CashInNotificationContext context)
        {
            await _webHookSender.ProcessCashIn(operationId: context.OperationId, dateTime: context.DateTime,
                walletId: context.WalletId, assetId: context.AssetId, amount: context.Amount,
                sourceAddress: context.SourceAddress);
        }


        [QueueTrigger(CashOutStartedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutStartedNotification(CashOutStartedNotificationContext context)
        {
            await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: context.DateTime,
                walletId: context.WalletId, assetId: context.AssetId, amount: context.Amount,
                destAddress: context.DestAddress, txHash:context.TxHash);
        }

        [QueueTrigger(CashOutCompletedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutCompletedNotification(CashOutCompletedNotificationContext context)
        {
            await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: context.DateTime,
                walletId: context.WalletId, assetId: context.AssetId, amount: context.Amount,
                destAddress: context.DestAddress, txHash: context.TxHash);
        }
    }
}
