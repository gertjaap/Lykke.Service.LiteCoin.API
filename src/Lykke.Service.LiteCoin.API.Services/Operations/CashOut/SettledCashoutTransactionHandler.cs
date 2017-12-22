using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashOut
{
    public class SettledCashoutTransactionHandler: ISettledCashoutTransactionHandler
    {
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly ILog _log;
        private readonly IQueueRouter<CashOutCompletedNotificationContext> _notificationsQueue;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ICashOutEventRepository _eventRepository;
        
        public SettledCashoutTransactionHandler(ICashOutOperationRepository cashOutOperationRepository, 
            ILog log, 
            IQueueRouter<CashOutCompletedNotificationContext> notificationsQueue,
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository, ICashOutEventRepository eventRepository)
        {
            _cashOutOperationRepository = cashOutOperationRepository;
            _log = log;
            _notificationsQueue = notificationsQueue;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _eventRepository = eventRepository;
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
            var operation = await _cashOutOperationRepository.GetByOperationId(tx.OperationId);

            if (operation == null)
            {
                //mb throw error on slack?
                await _log.WriteWarningAsync(nameof(SettledCashoutTransactionHandler), nameof(HandleSettledTransaction),
                    tx.ToJson(), $"Cashout operation {tx.OperationId} not found");

                return;
            }

            await _notificationsQueue.AddMessage(new CashOutCompletedNotificationContext
            {
                OperationId = operation.OperationId
            });


            await _pendingCashoutTransactionRepository.Remove(tx.TxHash);
            await _eventRepository.InsertEvent(CashOutEvent.Create(operation.OperationId,
                CashOutEventType.DetectedOnBlockChain));
        }
    }
}
