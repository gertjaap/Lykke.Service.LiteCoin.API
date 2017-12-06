using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.LiteCoin.API.Core.Address;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Address
{
    public class AddressValidator: IAddressValidator
    {
        public bool IsValid(string address)
        {
            try
            {
                BitcoinAddress.Create(address);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
