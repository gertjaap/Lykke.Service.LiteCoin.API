﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.CashOut;
using Lykke.Service.LiteCoin.API.Core.Queue;
using Lykke.Service.LiteCoin.API.Core.Queue.Contexts;
using Lykke.Service.LiteCoin.API.Services.CashOut;
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

            var op = CashOutOperation.Create("opid", "walletId", "add", 100, "asset", DateTime.UtcNow, "txHash");

            var cashOutRepo = GetCashOutOperationRepository(op);
            var queueRouter = GetQueueRouter(op);
            var txRepo = GetTrackedCashoutTxRepo(op);
            var eventRepo = GetCashOutEventRepository(op);

            var handler = new SettledCashoutTransactionHandler(cashOutRepo.Object, new EmptyLog(), queueRouter.Object,
                txRepo.Object, eventRepo.Object);

            await handler.HandleSettledTransactions(new[] {CashOutTransaction.Create(op.TxHash, op.OperationId)});

            cashOutRepo.Verify();
            queueRouter.Verify();
            txRepo.Verify();

        }

        private Mock<ICashOutOperationRepository> GetCashOutOperationRepository(ICashOutOperation op)
        {
            var result = new Mock<ICashOutOperationRepository>();

            result.Setup(p => p.GetByOperationId(It.Is<string>(x => x == op.OperationId)))
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

        private Mock<IQueueRouter<CashOutCompletedNotificationContext>> GetQueueRouter(ICashOutOperation op)
        {
            var result = new Mock<IQueueRouter<CashOutCompletedNotificationContext>>();

            Func<CashOutCompletedNotificationContext, bool> validateContext = p =>
            {

                if (p.OperationId == op.OperationId)

                {
                    return true;
                }

                return false;
            };
            result.Setup(p=>p.AddMessage(It.Is<CashOutCompletedNotificationContext>(x=>validateContext(x))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return result;
        }

        private Mock<IPendingCashoutTransactionRepository> GetTrackedCashoutTxRepo(ICashOutOperation op)
        {
            var result = new Mock<IPendingCashoutTransactionRepository>();
            
            result.Setup(p=>p.Remove(It.Is<string>(x=>x==op.TxHash)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return result;
        }
    }
}
