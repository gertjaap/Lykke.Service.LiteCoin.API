using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Broadcast
{
    public class BroadcastService:IBroadcastService
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ITransactionOutputsService _transactionOutputsService;

        public BroadcastService(IBlockChainProvider blockChainProvider, 
            ITransactionOutputsService transactionOutputsService)
        {
            _blockChainProvider = blockChainProvider;
            _transactionOutputsService = transactionOutputsService;
        }

        public async Task BroadCastTransaction(Transaction tx)
        {
            await _blockChainProvider.BroadCastTransaction(tx);
            await _transactionOutputsService.SaveOuputs(tx);
        }
    }
}
