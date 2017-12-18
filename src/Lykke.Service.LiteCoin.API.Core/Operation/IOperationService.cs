using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Operation
{

    public interface IOperationService
    {
        Task ProceedCashOutOperation(string operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount);
    }
}
