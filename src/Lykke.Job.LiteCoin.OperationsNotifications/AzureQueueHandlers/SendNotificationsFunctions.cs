using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.WebHook;

namespace Lykke.Job.LiteCoin.OperationsNotifications.AzureQueueHandlers
{
    public class SendNotificationsFunctions
    {
        private readonly IWebHookSender _webHookSender;
        private readonly ILog _log;

        public SendNotificationsFunctions(IWebHookSender webHookSender, ILog log)
        {
            _webHookSender = webHookSender;
            _log = log;
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

        [QueueTrigger(CashOutStartedNotificationContext.QueueName, notify: true)]
        public async Task SendCashOutCompletedNotification(CashOutStartedNotificationContext context)
        {
            await _webHookSender.ProcessCashOutStarted(operationId: context.OperationId, dateTime: context.DateTime,
                walletId: context.WalletId, assetId: context.AssetId, amount: context.Amount,
                destAddress: context.DestAddress, txHash: context.TxHash);
        }
    }
}
