using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs
{
    public interface ISpentOutputService
    {
        Task SaveSpentOutputs(Transaction transaction);
        
    }
}
