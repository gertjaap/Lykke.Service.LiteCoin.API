using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiteCoin.API.AzureRepositories.Helpers
{
    public static class AzureTableStoragePaginationHelper
    {
        public static async Task<IEnumerable<T>> GetPagedResult<T>(this INoSQLTableStorage<T> storage, string partition, int skip, int take) where T:ITableEntity, new()
        {
            var query = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition))
                .Take(take);

            var page = new AzureStorage.Tables.Paging.PagingInfo { ElementCount = skip };
            
            var result  = await storage.ExecuteQueryWithPaginationAsync(query, page);

            return result.ToList();
        } 
    }
}
