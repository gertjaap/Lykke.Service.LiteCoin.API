using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.Vertcoin.API.Core.ObservableOperation;
using Lykke.Service.Vertcoin.API.Core.Transactions;

namespace Lykke.Service.Vertcoin.API.Services.ObservableOperation
{
    public class ObservableOperationService: IObservableOperationService
    {
        private readonly IObservableOperationRepository _observableOperationRepository;
        private readonly IUnconfirmedTransactionRepository _unconfirmedTransactionRepository;

        public ObservableOperationService(IObservableOperationRepository observableOperationRepository, IUnconfirmedTransactionRepository unconfirmedTransactionRepository)
        {
            _observableOperationRepository = observableOperationRepository;
            _unconfirmedTransactionRepository = unconfirmedTransactionRepository;
        }
        
        public async Task DeleteOperations(params Guid[] opIds)
        {
            await _observableOperationRepository.DeleteIfExist(opIds);
            await _unconfirmedTransactionRepository.DeleteIfExist(opIds);
        }

        public Task<IObservableOperation> GetById(Guid opId)
        {
            return _observableOperationRepository.GetById(opId);
        }
    }
}
