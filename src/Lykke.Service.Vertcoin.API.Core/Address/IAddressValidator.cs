using NBitcoin;

namespace Lykke.Service.Vertcoin.API.Core.Address
{
    public interface IAddressValidator
    {
        bool IsValid(string address);
        BitcoinAddress GetBitcoinAddress(string address);
    }
}
