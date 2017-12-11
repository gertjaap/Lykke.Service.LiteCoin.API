using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Services.WebHook.Contracts;

namespace Lykke.Service.LiteCoin.API.Services.CashOut
{
    public class SettledCashoutTransactionHandler: ISettledCashoutTransactionHandler
    {
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly ILog _log;
        private readonly IQueueRouter<CashOutCompletedNotificationContext> _notificationsQueue;
        private readonly ITrackedCashoutTransactionRepository _trackedCashoutTransactionRepository;
        
        public SettledCashoutTransactionHandler(ICashOutOperationRepository cashOutOperationRepository, 
            ILog log, 
            IQueueRouter<CashOutCompletedNotificationContext> notificationsQueue,
            ITrackedCashoutTransactionRepository trackedCashoutTransactionRepository)
        {
            _cashOutOperationRepository = cashOutOperationRepository;
            _log = log;
            _notificationsQueue = notificationsQueue;
            _trackedCashoutTransactionRepository = trackedCashoutTransactionRepository;
        }

        public async Task HandleSettledTransactions(IEnumerable<ICashoutTransaction> settledTransactions)
        {
            foreach (var settledTransaction in settledTransactions)
            {
                await HandleSettledTransaction(settledTransaction);
            }
        }

        private async Task HandleSettledTransaction(ICashoutTransaction tx)
        {
            var operation = await _cashOutOperationRepository.Get(tx.OperationId);

            if (operation == null)
            {
                //mb throw error on slack?
                await _log.WriteWarningAsync(nameof(SettledCashoutTransactionHandler), nameof(HandleSettledTransaction),
                    tx.ToJson(), $"Cashout operation {tx.OperationId} not found");

                return;
            }

            await _notificationsQueue.AddMessage(new CashOutCompletedNotificationContext
            {
                Amount = operation.Amount,
                AssetId = operation.AssetId,
                DateTime = operation.StartedAt,
                DestAddress = operation.Address,
                OperationId = operation.OperationId,
                TxHash = operation.TxHash,
                WalletId = operation.WalletId
            });

            await _cashOutOperationRepository.SetCompleted(tx.OperationId, DateTime.UtcNow);

            await _trackedCashoutTransactionRepository.Remove(tx.TxHash);
        }
    }
}
