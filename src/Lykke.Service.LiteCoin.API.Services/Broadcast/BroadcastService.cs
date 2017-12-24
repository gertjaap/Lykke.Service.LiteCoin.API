using System;
using System.Threading.Tasks;
using Common.Log;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Broadcast;
using Lykke.Service.LiteCoin.API.Core.Exceptions;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Broadcast
{
    public class BroadcastService:IBroadcastService
    {
        private readonly IBlockChainProvider _blockChainProvider;
        private readonly ITransactionOutputsService _transactionOutputsService;
        private readonly ILog _log;

        public BroadcastService(IBlockChainProvider blockChainProvider, 
            ITransactionOutputsService transactionOutputsService, ILog log)
        {
            _blockChainProvider = blockChainProvider;
            _transactionOutputsService = transactionOutputsService;
            _log = log;
        }

        public async Task BroadCastTransaction(Transaction tx)
        {
            try
            {
                await _blockChainProvider.BroadCastTransaction(tx);
            }
            catch (FlurlHttpException e)
            {
                await _log.WriteErrorAsync(nameof(BroadcastService), nameof(BroadCastTransaction),
                    tx.GetHash().ToString(), e);
                throw new BusinessException($"Broadcast error - {e.Call.ErrorResponseBody}", ErrorCode.BroadcastError);
            }

            await _transactionOutputsService.SaveOuputs(tx);
        }
    }
}
