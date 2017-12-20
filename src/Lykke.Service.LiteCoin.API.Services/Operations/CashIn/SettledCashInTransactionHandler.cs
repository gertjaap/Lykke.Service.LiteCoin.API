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
        private readonly IQueueRouter<CashInNotificationContext> _webHookQueue;
        private readonly IQueueRouter<SendCashInToHotWalletContext> _sendCashInToHotWalletQueue;
        private readonly ICashInOperationRepository _cashInOperationRepository;

        public SettledCashInTransactionHandler(IQueueRouter<CashInNotificationContext> webHookQueue, 
            ICashInOperationRepository cashInOperationRepository, 
            IQueueRouter<SendCashInToHotWalletContext> sendCashInToHotWalletQueue)
        {
            _webHookQueue = webHookQueue;
            _cashInOperationRepository = cashInOperationRepository;
            _sendCashInToHotWalletQueue = sendCashInToHotWalletQueue;
        }

        public async Task HandleSettledTransactions(IEnumerable<ICashInOperation> settledTransactions)
        {
            foreach (var settledTransaction in settledTransactions)
            {
                await _cashInOperationRepository.Insert(settledTransaction);
                
                await _webHookQueue.AddMessage(new CashInNotificationContext
                {
                    OperationId = settledTransaction.OperationId
                });

                await _sendCashInToHotWalletQueue.AddMessage(new SendCashInToHotWalletContext
                {
                    OperationId = settledTransaction.OperationId,
                });
            }
        }
    }
}
