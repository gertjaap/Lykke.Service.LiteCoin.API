using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.Address
{
    public interface IAddressValidator
    {
        bool IsValid(string address);
        BitcoinAddress GetBitcoinAddress(string address);
    }
}
