using Lykke.Service.Vertcoin.API.Core.Address;
using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Services.Address
{
    public class AddressValidator: IAddressValidator
    {
        public bool IsValid(string address)
        {
            var addr = GetBitcoinAddress(address);

            return addr != null;
        }

        public BitcoinAddress GetBitcoinAddress(string address)
        {
            try
            {
                return BitcoinAddress.Create(address);
            }
            catch
            {
                return null;
            }
        }
    }
}
