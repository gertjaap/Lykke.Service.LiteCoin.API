using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Core.TransactionOutputs.SpentOutputs
{
    public class Output : IOutput
    {
        public string TransactionHash { get; set; }
        public int N { get; set; }

        public Output(OutPoint outPoint)
        {
            TransactionHash = outPoint.Hash.ToString();
            N = (int)outPoint.N;
        }
    }
}
