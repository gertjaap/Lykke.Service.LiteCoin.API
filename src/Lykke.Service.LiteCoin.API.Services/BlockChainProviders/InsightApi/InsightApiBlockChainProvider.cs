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
            var resp = await _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addr/{address}")
                .SetQueryParam("from", fromHeight)
                .SetQueryParam("to", toHeight)
                .GetJsonAsync<AddressBalanceResponceContract>();

            return resp.Transactions;
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
            try
            {
                var resp = await _insightApiSettings.Url
                    .AppendPathSegment($"insight-lite-api/tx/{txHash}")
                    .GetJsonAsync<TxResponceContract>();

                return resp.Confirmation;
            }
            catch (FlurlHttpException e) when (e.Call.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<Coin>> GetUnspentOutputs(string address, int minConfirmationCount)
        {
            var resp =await _insightApiSettings.Url
                .AppendPathSegment($"insight-lite-api/addr/{address}/utxo")
                .GetJsonAsync<AddressUnspentOutputsResponce[]>();

            return resp.Where(p => p.Confirmation >= minConfirmationCount).Select(MapUnspentCoun);
        }

        private Coin MapUnspentCoun(AddressUnspentOutputsResponce source)
        {
            return new Coin(new OutPoint(uint256.Parse(source.TxHash), source.N), new TxOut(new Money(source.Satoshi, MoneyUnit.Satoshi), source.ScriptPubKey.ToScript()));
        }
    }
}
