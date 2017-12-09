using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi
{
    internal class InsightApiBlockChainProvider: IBlockChainProvider
    {
        private readonly InsightApiSettings _insightApiSettings;

        public InsightApiBlockChainProvider(InsightApiSettings insightApiSettings)
        {
            _insightApiSettings = insightApiSettings;
        }

        public async Task<IEnumerable<string>> GetTransactionsForAddress(string address, int minBlockHeight)
        {
            var resp = await _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addr/{address}")
                .SetQueryParam("from", minBlockHeight)
                .GetJsonAsync<AddressBalanceResponceContract>();

            return resp.Transactions;
        }

        public async Task<int> GetLastBlockHeight()
        {
            var resp = await _insightApiSettings.Url
                .AppendPathSegment("insight-lite-api/status")
                .GetJsonAsync<StatusResponceContract>();

            return resp.Info.LastBlockHeight;
        }

        public async Task<Transaction> GetRawTx(string tx)
        {
            var resp = await _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/rawtx/{tx}")
                .GetJsonAsync<RawTxResponce>();

            return Transaction.Parse(resp.RawTx);
        }

        public async Task BroadCastTransaction(Transaction tx)
        {
            await _insightApiSettings.Url.AppendPathSegment("insight-lite-api/tx/send")
                .PostJsonAsync(new BroadcastTransactionRequestContract
                {
                    RawTx = tx.ToHex()
                })
                .ReceiveJson<BroadcastTransactionResponceContract>();
        }
    }
}
