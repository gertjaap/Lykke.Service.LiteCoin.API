using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Services.Operations.CashOut;
using Moq;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class SettledCashoutHandlerTests
    {
        [Fact]
        public async Task CanHandleCashoutOp()
        {


            var op = CashOutOperation.Create(Guid.NewGuid(), "walletId", "add", 100, "asset", DateTime.UtcNow, "txHash");

            var cashOutRepo = GetCashOutOperationRepository(op);
            var notificationTxRepo = GetNotificationTxRepo(op);
            var txRepo = GetTrackedCashoutTxRepo(op);
            var eventRepo = GetCashOutEventRepository(op);

            var handler = new SettledCashoutTransactionHandler(cashOutRepo.Object, new EmptyLog(),
                txRepo.Object, eventRepo.Object, notificationTxRepo.Object);

            await handler.HandleSettledTransactions(new[] {CashOutTransaction.Create(op.TxHash, op.OperationId)});

            cashOutRepo.Verify();
            notificationTxRepo.Verify();
            txRepo.Verify();
            notificationTxRepo.Verify();

        }

        private Mock<ICashOutOperationRepository> GetCashOutOperationRepository(ICashOutOperation op)
        {
            var result = new Mock<ICashOutOperationRepository>();

            result.Setup(p => p.GetByOperationId(It.Is<Guid>(x => x == op.OperationId)))
                .ReturnsAsync(op).Verifiable();

            return result;
        }

        private Mock<ICashOutEventRepository> GetCashOutEventRepository(ICashOutOperation op)
        {
            var result = new Mock<ICashOutEventRepository>();
            
            result.Setup(p => p.InsertEvent(It.Is<ICashOutEvent>(x => x.OperationId == op.OperationId && x.Type == CashOutEventType.DetectedOnBlockChain)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return result;
        }

        private Mock<IUnconfirmedCashoutTransactionRepository> GetTrackedCashoutTxRepo(ICashOutOperation op)
        {
            var result = new Mock<IUnconfirmedCashoutTransactionRepository>();
            
            result.Setup(p=>p.Remove(It.Is<string>(x=>x==op.TxHash)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return result;
        }

        private Mock<IPendingCashOutEventRepository> GetNotificationTxRepo(ICashOutOperation op)
        {
            var result = new Mock<IPendingCashOutEventRepository>();

            result.Setup(p => p.Insert(It.Is<IPendingCashOutEvent>(x => x.TxHash == op.TxHash && x.OperationId == op.OperationId)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return result;
        }
    }
}
