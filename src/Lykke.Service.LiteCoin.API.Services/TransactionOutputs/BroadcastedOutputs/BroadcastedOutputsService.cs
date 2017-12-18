using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.TransactionOutputs.BroadcastedOutputs
{
    public class BroadcastedOutputsService:IBroadcastedOutputsService
    {
        private readonly IBroadcastedOutputRepository _broadcastedOutputRepository;
        private readonly Network _network;

        public BroadcastedOutputsService(IBroadcastedOutputRepository broadcastedOutputRepository, Network network)
        {
            _broadcastedOutputRepository = broadcastedOutputRepository;
            _network = network;
        }

        public async Task SaveNewOutputs(Transaction tr)
        {
            await _broadcastedOutputRepository.InsertOutputs(
                tr.Outputs.AsCoins().Select(o => new BroadcastedOutput(o, tr.GetHash().ToString(), _network)).ToList());
        }
    }
}
