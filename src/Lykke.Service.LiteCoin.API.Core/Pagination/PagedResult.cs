using System.Collections.Generic;

namespace Lykke.Service.LiteCoin.API.Core.Pagination
{
    public interface IPadedResult<T>
    {
        IEnumerable<T> Items { get; }

        string Continuation { get; }
    }

    public class PagedResult<T> : IPadedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public string Continuation { get; set; }

        public static PagedResult<T> Create<T>(IEnumerable<T> items, string continuation)
        {
            return new PagedResult<T>
            {
                Continuation = continuation,
                Items = items
            };
        }
    }
}
