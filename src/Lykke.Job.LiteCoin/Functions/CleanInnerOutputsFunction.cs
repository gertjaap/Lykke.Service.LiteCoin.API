using System;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.BroadcastedOutputs;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Services.TransactionOutputs;

namespace Lykke.Job.LiteCoin.Functions
{
    public class CleanInnerOutputsFunction
    {
        private readonly ISpentOutputRepository _spentOutputRepository;
        private readonly TransactionOutputsExpirationSettings _settings;
        private readonly IBroadcastedOutputRepository _broadcastedOutputRepository;

        public CleanInnerOutputsFunction(ISpentOutputRepository spentOutputRepository, TransactionOutputsExpirationSettings settings, IBroadcastedOutputRepository broadcastedOutputRepository)
        {             
            _spentOutputRepository = spentOutputRepository;
            _settings = settings;
            _broadcastedOutputRepository = broadcastedOutputRepository;
        }


        [TimerTrigger("00:10:00")]
        public async Task CleanSpentOutputs()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.SpentOutputsExpirationMinutes);
            await _spentOutputRepository.DeleteOldOutputs(bound);
        }

        [TimerTrigger("00:10:00")]
        public async Task CleanBroadcastedOutputs()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.BroadcastedOutputsExpirationMinutes);

            await _broadcastedOutputRepository.DeleteOldOutputs(bound);
        }
    }
}
