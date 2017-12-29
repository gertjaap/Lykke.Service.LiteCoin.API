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
        private readonly IPendingCashInEventRepository _cashInEventRepository;

        public SettledCashInTransactionHandler(ICashInOperationRepository cashInOperationRepository, 
            IQueueRouter<SendCashInToHotWalletContext> sendCashInToHotWalletQueue, 
            IPendingCashInEventRepository cashInEventRepository)
        {
            _cashInOperationRepository = cashInOperationRepository;
            _sendCashInToHotWalletQueue = sendCashInToHotWalletQueue;
            _cashInEventRepository = cashInEventRepository;
        }

        public async Task HandleSettledTransactions(IEnumerable<ICashInOperation> cashInOperations)
        {
            foreach (var cashInOperation in cashInOperations)
            {
                await _cashInOperationRepository.Insert(cashInOperation);
                await _cashInEventRepository.Insert(PendingCashInEvent.Create(cashInOperation, PendingCashInEventStatusType.DetectedOnBlockChain));
                
                await _sendCashInToHotWalletQueue.AddMessage(new SendCashInToHotWalletContext
                {
                    OperationId = cashInOperation.OperationId,
                });
            }
        }
    }
}
