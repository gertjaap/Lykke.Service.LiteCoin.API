using System;
using Common.Log;

namespace Lykke.Service.LiteCoin.API.Client
{
    public class LiteCoin.APIClient : ILiteCoin.APIClient, IDisposable
    {
        private readonly ILog _log;

        public LiteCoin.APIClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
