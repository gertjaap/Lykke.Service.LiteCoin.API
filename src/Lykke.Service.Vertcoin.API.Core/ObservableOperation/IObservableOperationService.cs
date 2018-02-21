using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Vertcoin.API.Core.ObservableOperation
{
    public interface IObservableOperationService
    {
        Task DeleteOperations(params Guid[] opIds);
        Task<IObservableOperation> GetById(Guid opId);
    }
}
