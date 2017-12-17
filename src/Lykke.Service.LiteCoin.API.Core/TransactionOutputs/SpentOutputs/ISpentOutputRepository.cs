using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs
{
    public interface ISpentOutput: IOutput
    {

        DateTime InsertedAt { get; }
    }

    public interface IOutput
    {
        string TransactionHash { get; set; }

        int N { get; set; }
    }

    public interface ISpentOutputRepository
    {
        Task InsertSpentOutputs(IEnumerable<ISpentOutput> outputs);

        Task<IEnumerable<ISpentOutput>> GetUnspentOutputs(IEnumerable<IOutput> outputs);

        Task DeleteOldOutputs(DateTime boun);
    }
}
