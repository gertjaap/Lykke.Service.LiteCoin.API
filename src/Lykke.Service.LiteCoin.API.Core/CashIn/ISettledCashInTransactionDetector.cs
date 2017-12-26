using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Service.LiteCoin.API.Core.CashIn
{
    public interface IDetectCashInOperationsResult
    {
        IEnumerable<ICashInOperation> DetectedOperations { get;  }
        IEnumerable<IDetectedAddressTransaction> AddressTransactions { get;  }
    }

    public interface ISettledCashInTransactionDetector
    {
        Task<IDetectCashInOperationsResult> GetCashInOperations(IWallet wallet, IEnumerable<IDetectedAddressTransaction> prevDetectedTransactions, int minTxConfirmationCount);
    }
}
