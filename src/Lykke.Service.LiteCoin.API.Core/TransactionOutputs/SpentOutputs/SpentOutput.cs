using System;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs
{
    public class SpentOutput: ISpentOutput
    {
        public string TransactionHash { get; set; }
        public int N { get; set; }
        public DateTime InsertedAt { get; set; }

        public static SpentOutput Create(OutPoint outPoint, DateTime? insertedAt = null)
        {
            return new SpentOutput
            {
                TransactionHash = outPoint.Hash.ToString(),
                N = (int) outPoint.N,
                InsertedAt = insertedAt ?? DateTime.UtcNow
            };
        }
    }
}
