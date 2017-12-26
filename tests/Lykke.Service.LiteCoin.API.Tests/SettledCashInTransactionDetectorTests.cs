using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.LiteCoin.API.Core.BlockChainReaders;
using Lykke.Service.LiteCoin.API.Core.CashIn;
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

            var tx1 = Transaction.Parse(
                "01000000000101c6d2e15ed356190bf59697936cc117d616b31f952e581f2fc97493874d8ddc5b0100000023220020e22f83818f7f9ba32fc3435a0332e5704eda366e344be8234c6cad20e0ee205bffffffff0200e1f505000000001976a914ccb3ceb1a2f701f97e23fd67234114fcc3c7f01a88acc04fa9320000000017a914af95c82c75dd014588768591e5fc00f4b151b3a287040048304502210094cfc773199062faaa042a7881d749e21347a88ddab8abaa447610537caa8897022071c72d227559fe9949ed63046156e4963f2a498535b2c3fe935cccd542d3eadd01473044022047e46d77cdc8830dfe241a72ff4095e41cee25a0325dcdce1e8130443cc26e4a0220620d7cd5ae7eaa2c2378d0c22399537c7a50a2fe970fc71ca9ebf149b975bf5f0147522103de8d79e765ba4cb98d2f429715e949227fa100ac8d402d01b0c8eab9a421941a210214a4d039a10970c6518b68ae6aa9208028c61f88d65762bd8e3bb6655a360a6a52ae00000000");
            var tx2 = Transaction.Parse(
                "0100000000010192677a2d88c3fb673210fafeedd91611db59cf51b89b00c0b923febfa4e6429c0100000023220020e22f83818f7f9ba32fc3435a0332e5704eda366e344be8234c6cad20e0ee205bffffffff0280f0fa02000000001976a9145a91b542ccd6916194825d26b8b893457e16ee8c88ac20859f380000000017a914af95c82c75dd014588768591e5fc00f4b151b3a2870400483045022100f13eeb3136cf6ed06e4ba78377f5ad188d3ca3ae7394b897014f4f4db7669011022039ddff2c3e7c5c819462e76b6182e8fba0575da9d5c6130933bcef9c57bc880701483045022100f598232d0914fb7736be1b267e95123185e551a96dabcf2813cc091e6e33c831022051d46d64fbfe6d83da8af7e1640516bcec2488a044dd4e9153c7dc95464df4760147522103de8d79e765ba4cb98d2f429715e949227fa100ac8d402d01b0c8eab9a421941a210214a4d039a10970c6518b68ae6aa9208028c61f88d65762bd8e3bb6655a360a6a52ae00000000");
            var addr = BitcoinAddress.Create("mzBKYRa9Kcpt6bGXopt8iRKi4heEkbdR8n", network);
            var sourceAddress = "QccPnuA18HA7CM8dBuZnWGHKNYsBFJJXX8";
            var blockchainProvider = GetBlockChainProvider(addr, sourceAddress, "5bdc8d4d879374c92f1f582e951fb316d617c16c939796f50b1956d35ee1d2c6", 1, tx1, tx2);

            var detector = new SettledCashInTransactionDetector(blockchainProvider.Object, network, new EmptyLog());

            var ops = await detector.GetCashInOperations(WalletMock.Create(addr), Enumerable.Empty<IDetectedAddressTransaction>(), 0);

            Assert.True(ops.DetectedOperations.Count() == 1);

            var op = ops.DetectedOperations.First();

            Assert.True(op.SourceAddress == sourceAddress);

            Assert.True(op.TxHash == tx1.GetHash().ToString());
            
            Assert.True(op.Amount == new Money(100000000).ToUnit(MoneyUnit.BTC));


            Assert.True(op.AssetId == Constants.AssetsContants.LiteCoin);


            Assert.True(op.DestinationAddress == addr.ToString());
            blockchainProvider.Verify();
        }

        [Fact]
        public async Task NotDetectAlreadyProcessedTransactions()
        {
            PrepareNetworks.EnsureLiteCoinPrepared();
            var network = Network.TestNet;

            var tx1 = Transaction.Parse(
                "01000000000101c6d2e15ed356190bf59697936cc117d616b31f952e581f2fc97493874d8ddc5b0100000023220020e22f83818f7f9ba32fc3435a0332e5704eda366e344be8234c6cad20e0ee205bffffffff0200e1f505000000001976a914ccb3ceb1a2f701f97e23fd67234114fcc3c7f01a88acc04fa9320000000017a914af95c82c75dd014588768591e5fc00f4b151b3a287040048304502210094cfc773199062faaa042a7881d749e21347a88ddab8abaa447610537caa8897022071c72d227559fe9949ed63046156e4963f2a498535b2c3fe935cccd542d3eadd01473044022047e46d77cdc8830dfe241a72ff4095e41cee25a0325dcdce1e8130443cc26e4a0220620d7cd5ae7eaa2c2378d0c22399537c7a50a2fe970fc71ca9ebf149b975bf5f0147522103de8d79e765ba4cb98d2f429715e949227fa100ac8d402d01b0c8eab9a421941a210214a4d039a10970c6518b68ae6aa9208028c61f88d65762bd8e3bb6655a360a6a52ae00000000");

            var addr = BitcoinAddress.Create("mzBKYRa9Kcpt6bGXopt8iRKi4heEkbdR8n", network);
            var sourceAddress = "QccPnuA18HA7CM8dBuZnWGHKNYsBFJJXX8";
            var blockchainProvider = GetBlockChainProvider(addr, sourceAddress, "5bdc8d4d879374c92f1f582e951fb316d617c16c939796f50b1956d35ee1d2c6", 1, tx1);

            var detector = new SettledCashInTransactionDetector(blockchainProvider.Object, network, new EmptyLog());

            var ops = await detector.GetCashInOperations(WalletMock.Create(addr), new []{ DetectedAddressTransaction.Create(tx1.GetHash().ToString(), addr.ToString())}, 0);

            Assert.True(!ops.DetectedOperations.Any());
        }


        private Mock<IBlockChainProvider> GetBlockChainProvider(BitcoinAddress address, string sourceAddress,  string prevTxHash, uint prevN, params Transaction[] txs)
        {
            var result = new Mock<IBlockChainProvider>();

            result.Setup(p =>p.GetTransactionsForAddress(It.Is<BitcoinAddress>(x => x == address)))
                .ReturnsAsync(txs.Select(x=>x.GetHash().ToString()))
                .Verifiable();


            result.Setup(p => p.GetRawTx(It.IsIn(txs.Select(x => x.GetHash().ToString()))))
                .Returns<string>(p => Task.FromResult(txs.First(x => x.GetHash().ToString() == p)))
                .Verifiable();



            result.Setup(p => p.GetDestinationAddress(It.IsAny<string>(), It.IsAny<uint>()))
                .ReturnsAsync(sourceAddress)
                .Verifiable();

            return result;
        }

        internal class WalletMock:IWallet
        {
            public BitcoinAddress Address { get; set; }
            public bool IsClientWallet => true;

            public static WalletMock Create(BitcoinAddress addr)
            {
                return new WalletMock
                {
                    Address = addr
                };
            }
        }
    }
}
