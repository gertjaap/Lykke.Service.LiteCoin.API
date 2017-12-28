using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Operation
{

    public interface IOperationService
    {
        Task<ICashOutOperation> ProceedCashOutOperation(Guid operationId, IWallet sourceWallet, BitcoinAddress destAddress, decimal amount);
        Task ProceedSendMoneyToHotWalletOperation(Guid operationId, IWallet sourceWallet, string thHash);
    }
}
