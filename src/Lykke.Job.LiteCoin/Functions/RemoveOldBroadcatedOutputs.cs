using System;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Services.TransactionOutputs;

namespace Lykke.Job.LiteCoin.Functions
{
    public class RemoveOldBroadcatedOutputs
    {
        private readonly IBroadcastedOutputRepository _broadcastedOutputRepository;
        private readonly TransactionOutputsExpirationSettings _settings;

        public RemoveOldBroadcatedOutputs(IBroadcastedOutputRepository broadcastedOutputRepository, TransactionOutputsExpirationSettings settings)
        {
            _broadcastedOutputRepository = broadcastedOutputRepository;
            _settings = settings;
        }




        [TimerTrigger("1.00:00:00")]
        public async Task Clean()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.BroadcastedOutputsExpirationDays);

            await _broadcastedOutputRepository.DeleteOldOutputs(bound);
        }
    }
}

