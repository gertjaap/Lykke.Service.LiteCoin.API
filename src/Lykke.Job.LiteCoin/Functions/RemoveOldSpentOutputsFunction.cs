using System;
using System.Threading.Tasks;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs;
using Lykke.Service.LiteCoin.API.Services.TransactionOutputs;

namespace Lykke.Job.LiteCoin.Functions
{
    public class RemoveOldSpentOutputsFunction
    {
        private readonly ISpentOutputRepository _spentOutputRepository;
        private readonly TransactionOutputsExpirationSettings _settings;

        public RemoveOldSpentOutputsFunction(ISpentOutputRepository spentOutputRepository, TransactionOutputsExpirationSettings settings)
        {             
            _spentOutputRepository = spentOutputRepository;
            _settings = settings;
        }


        [TimerTrigger("1.00:00:00")]
        public async Task Clean()
        {
            var bound = DateTime.UtcNow.AddDays(-_settings.SpentOutputsExpirationDays);
            await _spentOutputRepository.DeleteOldOutputs(bound);
        }
    }
}
