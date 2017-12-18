using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.CashIn;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs;
using Lykke.Service.LiteCoin.API.Core.Wallet;

namespace Lykke.Job.LiteCoin.Functions
{
    public class SendCashInToHotWalletFunctions
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionOutputsService _outputsService;
        private readonly ICashInOperationRepository _cashInOperationRepository;

        public SendCashInToHotWalletFunctions(IWalletService walletService, ITransactionOutputsService outputsService, ICashInOperationRepository cashInOperationRepository)
        {
            _walletService = walletService;
            _outputsService = outputsService;
            _cashInOperationRepository = cashInOperationRepository;
        }

        //[QueueTrigger(SendCashInToHotWalletContext.QueueName)]
        //public async Task Send(SendCashInToHotWalletContext context)
        //{
        //    _cashInOperationRepository
        //    var hotWallet = (await _walletService.GetHotWallets()).First();

        //}
    }
}
