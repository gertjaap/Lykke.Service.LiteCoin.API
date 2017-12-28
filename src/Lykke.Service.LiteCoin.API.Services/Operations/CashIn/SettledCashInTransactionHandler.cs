using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class SettledCashInTransactionHandler: ISettledCashInTransactionHandler
    {
        private readonly IQueueRouter<SendCashInToHotWalletContext> _sendCashInToHotWalletQueue;
        private readonly ICashInOperationRepository _cashInOperationRepository;
        private readonly IPendingCashInNotificationRepository _cashInNotificationRepository;

        public SettledCashInTransactionHandler(ICashInOperationRepository cashInOperationRepository, 
            IQueueRouter<SendCashInToHotWalletContext> sendCashInToHotWalletQueue, 
            IPendingCashInNotificationRepository cashInNotificationRepository)
        {
            _cashInOperationRepository = cashInOperationRepository;
            _sendCashInToHotWalletQueue = sendCashInToHotWalletQueue;
            _cashInNotificationRepository = cashInNotificationRepository;
        }

        public async Task HandleSettledTransactions(IEnumerable<ICashInOperation> cashInOperations)
        {
            foreach (var cashInOperation in cashInOperations)
            {
                await _cashInOperationRepository.Insert(cashInOperation);
                await _cashInNotificationRepository.Insert(PendingCashInNotification.Create(cashInOperation));
                
                await _sendCashInToHotWalletQueue.AddMessage(new SendCashInToHotWalletContext
                {
                    OperationId = cashInOperation.OperationId,
                });
            }
        }
    }
}
