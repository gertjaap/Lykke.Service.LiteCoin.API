using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.CashOut;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashOut
{
    public class SettledCashoutTransactionHandler: ISettledCashoutTransactionHandler
    {
        private readonly ICashOutOperationRepository _cashOutOperationRepository;
        private readonly ILog _log;
        private readonly IPendingCashoutTransactionRepository _pendingCashoutTransactionRepository;
        private readonly ICashOutEventRepository _eventRepository;
        private readonly IPendingCashOutEventRepository _cashOutNotificationRepository;
        
        public SettledCashoutTransactionHandler(ICashOutOperationRepository cashOutOperationRepository, 
            ILog log, 
            IPendingCashoutTransactionRepository pendingCashoutTransactionRepository,
            ICashOutEventRepository eventRepository,
            IPendingCashOutEventRepository cashOutNotificationRepository)
        {
            _cashOutOperationRepository = cashOutOperationRepository;
            _log = log;
            _pendingCashoutTransactionRepository = pendingCashoutTransactionRepository;
            _eventRepository = eventRepository;
            _cashOutNotificationRepository = cashOutNotificationRepository;
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

            await _cashOutNotificationRepository.Insert(PendingCashOutEvent.Create(operation, PendingCashOutEventStatusType.Completed));
            await _pendingCashoutTransactionRepository.Remove(tx.TxHash);

            await _eventRepository.InsertEvent(CashOutEvent.Create(operation.OperationId,
                CashOutEventType.DetectedOnBlockChain));
        }
    }
}
