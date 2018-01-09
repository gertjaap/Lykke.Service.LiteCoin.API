using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Transactions
{
    public interface ITransactionBuilderService
    {
        Task<Transaction> GetTransferTransaction(BitcoinAddress source, BitcoinAddress destination, Money amount, bool includeFee);

    }
}
