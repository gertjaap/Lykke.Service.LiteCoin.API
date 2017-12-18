using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.Constants;
using Lykke.Service.LiteCoin.API.Core.Wallet;
using Lykke.Service.LiteCoin.API.Services.Operations.CashIn;
using Moq;
using NBitcoin;
using Xunit;

namespace Lykke.Service.LiteCoin.API.Tests
{
    public class SettledCashInTransactionDetectorTests
    {
        [Fact]
        public async Task CanDetectCashIn()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();
            var network = Network.TestNet;

            var tx = Transaction.Parse(
                "01000000000101c6d2e15ed356190bf59697936cc117d616b31f952e581f2fc97493874d8ddc5b0100000023220020e22f83818f7f9ba32fc3435a0332e5704eda366e344be8234c6cad20e0ee205bffffffff0200e1f505000000001976a914ccb3ceb1a2f701f97e23fd67234114fcc3c7f01a88acc04fa9320000000017a914af95c82c75dd014588768591e5fc00f4b151b3a287040048304502210094cfc773199062faaa042a7881d749e21347a88ddab8abaa447610537caa8897022071c72d227559fe9949ed63046156e4963f2a498535b2c3fe935cccd542d3eadd01473044022047e46d77cdc8830dfe241a72ff4095e41cee25a0325dcdce1e8130443cc26e4a0220620d7cd5ae7eaa2c2378d0c22399537c7a50a2fe970fc71ca9ebf149b975bf5f0147522103de8d79e765ba4cb98d2f429715e949227fa100ac8d402d01b0c8eab9a421941a210214a4d039a10970c6518b68ae6aa9208028c61f88d65762bd8e3bb6655a360a6a52ae00000000");

            var addr = "mzBKYRa9Kcpt6bGXopt8iRKi4heEkbdR8n";

            var blockchainProvider = GetBlockChainProvider("mzBKYRa9Kcpt6bGXopt8iRKi4heEkbdR8n", tx);

            var detector = new SettledCashInTransactionDetector(blockchainProvider.Object, network);
            var walletId = "walletId";

            var ops = await detector.GetCashInOperations(new[] {WalletMock.Create(addr, walletId) }, 1, 10);

            Assert.True(ops.Count() == 1);

            var op = ops.First();

            Assert.True(op.Address == addr);

            Assert.True(op.TxHash == tx.GetHash().ToString());
            
            Assert.True(op.Amount == new Money(100000000).ToUnit(MoneyUnit.BTC));


            Assert.True(op.AssetId == Constants.AssetsContants.LiteCoin);


            Assert.True(op.WalletId == walletId);
            
        }

        private Mock<IBlockChainProvider> GetBlockChainProvider(string address, params Transaction[] txs)
        {
            var result = new Mock<IBlockChainProvider>();

            result.Setup(p =>p.GetTransactionsForAddress(It.Is<string>(x => x == address), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(txs.Select(x=>x.GetHash().ToString()))
                .Verifiable();


            result.Setup(p => p.GetRawTx(It.IsIn(txs.Select(x => x.GetHash().ToString()))))
                .Returns<string>(p => Task.FromResult(txs.First(x => x.GetHash().ToString() == p)))
                .Verifiable();

            return result;
        }

        internal class WalletMock:IWallet
        {
            public string Address { get; set; }
            public string WalletId { get; set; }

            public static WalletMock Create(string addr, string walletId)
            {
                return new WalletMock
                {
                    Address = addr,
                    WalletId = walletId
                };
            }
        }
    }
}
