using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.SourceWallets;
using Lykke.Service.LiteCoin.API.Core.TrackedEntites;
using Lykke.Service.LiteCoin.API.Services.SourceWallet;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.SignFacade
{
    public class SignFacadeService: ISignFacadeService
    {
        private readonly SourceWalletsSettings _sourceWalletsSettings;

        public SignFacadeService(SourceWalletsSettings sourceWalletsSettings)
        {
            _sourceWalletsSettings = sourceWalletsSettings;
        }

        public Task<string> GetAddressesToTrack()
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> SignTransaction(Transaction unsignedTransaction, params BitcoinAddress[] signUsingAddresses)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetSourceWalletsBlockChainAddresses()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetSourceWalletsIds()
        {
            return Task.FromResult(_sourceWalletsSettings.SourceWalletIds);
        }
    }
}
