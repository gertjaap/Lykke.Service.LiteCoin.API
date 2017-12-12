using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;

namespace Lykke.Service.LiteCoin.API.Services.Operations.CashIn
{
    public class SettledCashInTransactionHandler: ISettledCashInTransactionHandler
    {
        private readonly IQueueRouter<CashInNotificationContext> _webHookQueue;
        private readonly ICashInOperationRepository _cashInOperationRepository;

        public SettledCashInTransactionHandler(IQueueRouter<CashInNotificationContext> webHookQueue, 
            ICashInOperationRepository cashInOperationRepository)
        {
            _webHookQueue = webHookQueue;
            _cashInOperationRepository = cashInOperationRepository;
        }

        public async Task HandleSettledTransactions(IEnumerable<ICashInOperation> settledTransactions)
        {
            foreach (var settledTransaction in settledTransactions)
            {
                await _cashInOperationRepository.Insert(settledTransaction);

                await _webHookQueue.AddMessage(new CashInNotificationContext
                {
                    AmountSatoshi = settledTransaction.AmountSatoshi,
                    AssetId = settledTransaction.AssetId,
                    DateTime = settledTransaction.DetectedAt,
                    OperationId = settledTransaction.OperationId,
                    SourceAddress = settledTransaction.Address,
                    WalletId = settledTransaction.WalletId
                });
            }
        }
    }
}
