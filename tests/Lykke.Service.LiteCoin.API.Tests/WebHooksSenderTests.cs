using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.WebHook;
using Lykke.Service.LiteCoin.API.Services.WebHook;
using Moq;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class WebHooksSenderTests
    {

        [Fact]
        public async Task SavesEventOnCashInWebHookFail()
        {
            var webHookSettings = new WebHookSettings
            {
                Url = "http://127.0.0.1/"
            };

            var opId = Guid.NewGuid().ToString();
            var failedEventRepo = CreateFailedEventsRepo(opId);

            var whSender = new WebHookSender(webHookSettings, new EmptyLog(), failedEventRepo.Object);


            await whSender.ProcessCashIn(opId, DateTime.UtcNow, "walletId", "assetId", 123, "asda");



            failedEventRepo.Verify();

        }

        [Fact]
        public async Task SavesEventOnCashOutCompletedWebHookFail()
        {
            var webHookSettings = new WebHookSettings
            {
                Url = "http://127.0.0.1/"
            };

            var opId = Guid.NewGuid().ToString();
            var failedEventRepo = CreateFailedEventsRepo(opId);

            var whSender = new WebHookSender(webHookSettings, new EmptyLog(), failedEventRepo.Object);

            await whSender.ProcessCashOutCompleted(opId, DateTime.UtcNow, "walletId", "assetId", 123, "asda", "txhash");

            failedEventRepo.Verify();

        }

        [Fact]
        public async Task SavesEventOnCashOutStartedWebHookFail()
        {
            var webHookSettings = new WebHookSettings
            {
                Url = "http://127.0.0.1/"
            };
            var opId = Guid.NewGuid().ToString();
            var failedEventRepo = CreateFailedEventsRepo(opId);

            var whSender = new WebHookSender(webHookSettings, new EmptyLog(), failedEventRepo.Object);

            await whSender.ProcessCashOutStarted(opId, DateTime.UtcNow, "walletId", "assetId", 123, "asda", "txhash");


            failedEventRepo.Verify();

        }




        private static Mock<IFailedWebHookEventRepository> CreateFailedEventsRepo(string operationId)
        {
            
            var repo = new Mock<IFailedWebHookEventRepository>();


            repo.Setup(x => x.Insert(It.Is<IFailedWebHookEvent>(p=> p !=null && p.OperationId == operationId)))
                .Returns(Task.CompletedTask)
                .Verifiable();
            return repo;
        }
    }
}
