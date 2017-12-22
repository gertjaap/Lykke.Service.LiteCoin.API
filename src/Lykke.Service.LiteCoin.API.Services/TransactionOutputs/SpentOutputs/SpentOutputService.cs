using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs.SpentOutputs
{
    public class SpentOutputService : ISpentOutputService
    {
        private readonly ISpentOutputRepository _spentOutputRepository;
        private readonly IBroadcastedOutputRepository _broadcastedOutputRepository;

        public SpentOutputService(ISpentOutputRepository spentOutputRepository, IBroadcastedOutputRepository broadcastedOutputRepository)
        {
            _spentOutputRepository = spentOutputRepository;
            _broadcastedOutputRepository = broadcastedOutputRepository;
        }

        public async Task SaveSpentOutputs(Transaction transaction)
        {
            await _spentOutputRepository.InsertSpentOutputs(transaction.Inputs.Select(o => SpentOutput.Create(o.PrevOut)));
            var tasks = new List<Task>();
            foreach (var outPoint in transaction.Inputs.Select(o => o.PrevOut))
                tasks.Add(_broadcastedOutputRepository.DeleteOutput(outPoint.Hash.ToString(), (int)outPoint.N));
            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<ISpentOutput>> GetUnspentOutputs(IEnumerable<IOutput> outputs)
        {
            return await _spentOutputRepository.GetUnspentOutputs(outputs);;
        }
    }
}
