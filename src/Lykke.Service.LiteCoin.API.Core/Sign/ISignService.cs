using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Sign
{
    public interface ISignService
    {
        Task<Transaction> SignTransaction(Transaction unsignedTransaction, params BitcoinAddress[] publicAddress);
    }
}
