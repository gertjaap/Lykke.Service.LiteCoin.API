using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs
{
    public interface IBroadcastedOutput
    {
        string Address { get; }

        string ScriptPubKey { get; }
        
        long Amount { get; }
        string TransactionHash { get;  }

        int N { get;  }

        DateTime InsertedAt { get; }

    }

    public interface IBroadcastedOutputRepository
    {
        Task InsertOutputs(IEnumerable<IBroadcastedOutput> outputs);
        Task<IEnumerable<IBroadcastedOutput>> GetOutputs(string address);
        Task DeleteOldOutputs(DateTime boun);
    }
}
