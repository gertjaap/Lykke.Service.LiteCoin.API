using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using NBitcoin;

namespace Lykke.Service.LiteCoin.API.Services.Transactions
{
    public class TransactionBuildContextFactory
    {
        private readonly IComponentContext _context;

        public TransactionBuildContextFactory(IComponentContext context)
        {
            _context = context;
        }

        public TransactionBuildContext Create()
        {
            return _context.Resolve<TransactionBuildContext>();
        }
    }
}
