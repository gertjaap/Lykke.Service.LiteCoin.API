using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Services.Operations.CashOut;
using Moq;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class CashoutSettlementDetectorTests
    {
        [Fact]
        public async Task CanDetectTransactions()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var tx = CashOutTransaction.Create("txHash", "operationId");

            var txConfirmationCount = 3;

            var provider = GetBlockChainProvider(tx, txConfirmationCount);

            var txDetector = new SettledCashOutTransactionDetector(provider.Object, new EmptyLog());

            var detectedTx = await txDetector.CheckSettlement(new[] {tx}, minConfirmationsCount: 1);

            Assert.True(detectedTx.Count() == 1);


            Assert.True(detectedTx.First().TxHash == tx.TxHash);


            Assert.True(detectedTx.First().OperationId == tx.OperationId);
        }

        [Fact]
        public async Task DoNotDetectTxOnLowerThanTrashHoldConfirmations()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();


            var tx = CashOutTransaction.Create("txHash", "operationId");

            var txConfirmationCount = 3;

            var provider = GetBlockChainProvider(tx, txConfirmationCount);

            var txDetector = new SettledCashOutTransactionDetector(provider.Object, new EmptyLog());

            var detectedTx = await txDetector.CheckSettlement(new[] { tx }, minConfirmationsCount: 10);

            Assert.True(!detectedTx.Any());
            
        }

        private Mock<IBlockChainProvider> GetBlockChainProvider(ICashoutTransaction tx, int confirmationCount)
        {
            var result = new Mock<IBlockChainProvider>();

            result.Setup(p => p.GetTxConfirmationCount(It.Is<string>(x => x == tx.TxHash)))
                .ReturnsAsync(confirmationCount).Verifiable();

            return result;
        }

    }
    
}
