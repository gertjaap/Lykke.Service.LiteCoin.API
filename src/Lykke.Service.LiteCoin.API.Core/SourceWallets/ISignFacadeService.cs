using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.SourceWallets
{
    public interface ISignFacadeService
    {
        Task<IEnumerable<string>> GetSourceWalletsBlockChainAddresses();
        Task<IEnumerable<string>> GetSourceWalletsIds();

        Task<string> GetAddressesToTrack();

        Task<Transaction> SignTransaction(Transaction unsignedTransaction, params BitcoinAddress[] signUsingAddresses);
    }
}
