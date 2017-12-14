using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs
{
    public interface ISpentOutput
    {
        string TransactionHash { get; set; }

        int N { get; set; }

        DateTime InsertedAt { get; }
    }

    public interface ISpentOutputRepository
    {
        Task InsertSpentOutputs(IEnumerable<ISpentOutput> outputs);

        Task<IEnumerable<ISpentOutput>> GetUnspentOutputs(IEnumerable<ISpentOutput> outputs);

        Task DeleteOldOutputs(DateTime boun);
    }
}
