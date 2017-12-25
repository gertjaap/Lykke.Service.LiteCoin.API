using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.Helpers;
using Lykke.Service.LiteCoin.API.Services.BlockChainProviders.InsightApi.Contracts;
using MoreLinq;
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

        public async Task<IEnumerable<string>> GetTransactionsForAddress(string address, int fromHeight, int toHeight)
        {
            var result = new List<string>();

            //insight api have limitaion 
            //"from" and "to" range should be less than or equal to 1000
            var batchSize = 1000;
            var blocksEnum = Enumerable.Range(fromHeight, toHeight - fromHeight + 1);

            foreach (var heightBatch in blocksEnum.Batch(batchSize, x => x.ToList()))
            {
                var resp = await _insightApiSettings.Url
                    .AppendPathSegment($"insight-lite-api/addr/{address}")
                    .SetQueryParam("from", heightBatch.First())
                    .SetQueryParam("to", heightBatch.Last())
                    .GetJsonAsync<AddressBalanceResponceContract>();

                result.AddRange(resp.Transactions);
            }

            return result;
        }

        public Task<IEnumerable<string>> GetTransactionsForAddress(BitcoinAddress address, int fromHeight, int toHeight)
        {
            return GetTransactionsForAddress(address.ToString(), fromHeight, toHeight);
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

        public async Task<int> GetTxConfirmationCount(string txHash)
        {
            return (await GetTx(txHash))?.Confirmation ?? 0;
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int minConfirmationCount)
        {
            var resp =await _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addr/{address}/utxo")
                .GetJsonAsync<AddressUnspentOutputsResponce[]>();

            return resp.Where(p => p.Confirmation >= minConfirmationCount).Select(MapUnspentCoun);
        }

        public async Task<string> GetDestinationAddress(string txHash, uint n)
        {
            return (await GetTx(txHash))?.Outputs?.FirstOrDefault(p => p.N == n)?.ScriptPubKey?.Addresses?.FirstOrDefault();
        }

        private Coin MapUnspentCoun(AddressUnspentOutputsResponce source)
        {
            return new Coin(new OutPoint(uint256.Parse(source.TxHash), source.N), new TxOut(new Money(source.Satoshi, MoneyUnit.Satoshi), source.ScriptPubKey.ToScript()));
        }

        private async Task<TxResponceContract> GetTx(string txHash)
        {
            try
            {
                var resp = await _insightApiSettings.Url
                    .AppendPathSegment($"insight-lite-api/tx/{txHash}")
                    .GetJsonAsync<TxResponceContract>();

                return resp;
            }
            catch (FlurlHttpException e) when (e.Call.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
