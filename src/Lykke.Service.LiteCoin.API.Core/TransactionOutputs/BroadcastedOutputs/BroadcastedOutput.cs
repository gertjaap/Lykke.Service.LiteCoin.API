using System;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs
{
    public class BroadcastedOutput : IBroadcastedOutput
    {

        public string Address { get; set; }

        public string ScriptPubKey { get; set; }

        public long Amount { get; set; }

        public string TransactionHash { get; set; }
        public int N { get; set; }
        public DateTime InsertedAt { get; set; }


        private BroadcastedOutput(ICoin coin, Network net, DateTime? insertedAt = null)
        {
            Address = coin.TxOut.ScriptPubKey.GetDestinationAddress(net).ToString();
            ScriptPubKey = coin.TxOut.ScriptPubKey.ToHex();
            N = (int)coin.Outpoint.N;
            var coin1 = coin as Coin;

            if (coin1 != null)
                Amount = coin1.Amount.Satoshi;

            InsertedAt = insertedAt ?? DateTime.UtcNow;
        }

        public BroadcastedOutput(ICoin coin, string transactionHash, Network net, DateTime? insertedAt = null) : this(coin, net, insertedAt)
        {

            TransactionHash = transactionHash;
        }
    }
}
