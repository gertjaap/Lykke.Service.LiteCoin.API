﻿using System.Threading.Tasks;

namespace Lykke.Service.Vertcoin.API.Core.Fee
{

    public interface IFeeRateFacade
    {
        Task<int> GetFeePerByte();
    }
}
