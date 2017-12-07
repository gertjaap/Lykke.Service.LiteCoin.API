using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Services.BlockChainReaders.InsightApi.Contracts;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.BlockChainReaders.InsightApi
{
    internal class InsightApiBlockChainReader: IBlockChainReader
    {
        private readonly InsightApiSettings _insightApiSettings;

        public InsightApiBlockChainReader(InsightApiSettings insightApiSettings)
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
    }
}
