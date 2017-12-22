using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.LiteCoin.API.Core.Transactions;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Transactions
{
    public class TransactionBlobStorage : ITransactionBlobStorage
    {
        private const string BlobContainer = "transactions";

        private readonly IBlobStorage _blobStorage;

        public TransactionBlobStorage(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<string> GetTransaction(string transactionId, TransactionBlobType type)
        {
            var key = GenerateKey(transactionId, type);
            if (await _blobStorage.HasBlobAsync(BlobContainer, key))
                return await _blobStorage.GetAsTextAsync(BlobContainer, key);
            return null;
        }

        public async Task AddOrReplaceTransaction(string transactionId, TransactionBlobType type, string transactionHex)
        {
            var key = GenerateKey(transactionId, type);
            if (await _blobStorage.HasBlobAsync(BlobContainer, key))
                await _blobStorage.DelBlobAsync(BlobContainer, key);
            await _blobStorage.SaveBlobAsync(BlobContainer, key, Encoding.UTF8.GetBytes(transactionHex));
        }

        private string GenerateKey(string transactionId, TransactionBlobType type)
        {
            return $"{transactionId}.{type}.txt";
        }
    }
}
