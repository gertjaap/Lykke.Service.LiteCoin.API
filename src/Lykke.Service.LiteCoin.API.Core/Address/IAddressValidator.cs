using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.LiteCoin.API.Core.Address
{
    public interface IAddressValidator
    {
        bool IsValid(string address);
    }
}
