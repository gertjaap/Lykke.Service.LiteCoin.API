using NBitcoin.DataEncoders;
using System.Linq;
using NBitcoin.Protocol;
using System;
using System.Net;
using System.Collections.Generic;
using NBitcoin.RPC;
using System.IO;

namespace NBitcoin.Vertcoin
{
	public class Networks
	{
		//Format visual studio
		//{({.*?}), (.*?)}
		//Tuple.Create(new byte[]$1, $2)
		static Tuple<byte[], int>[] pnSeed6_test = {
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x68,0xec,0xd3,0xce}, 15889),
			Tuple.Create(new byte[]{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0x42,0xb2,0xb6,0x23}, 15889)
		};

		[Obsolete("Use EnsureRegistered instead")]
		public static void Register()
		{
			EnsureRegistered();
		}
		public static void EnsureRegistered()
		{
			if(_LazyRegistered.IsValueCreated)
				return;
			// This will cause RegisterLazy to evaluate
			new Lazy<object>[] { _LazyRegistered }.Select(o => o.Value != null).ToList();
		}
		static Lazy<object> _LazyRegistered = new Lazy<object>(RegisterLazy, false);

		private static object RegisterLazy()
		{
			var port = 5889;
			NetworkBuilder builder = new NetworkBuilder();
			_Mainnet = builder.SetConsensus(new Consensus()
			{
				SubsidyHalvingInterval = 840000,
				MajorityEnforceBlockUpgrade = 750,
				MajorityRejectBlockOutdated = 950,
				MajorityWindow = 1000,
				PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				PowTargetTimespan = TimeSpan.FromSeconds(3.5 * 24 * 60 * 60),
				PowTargetSpacing = TimeSpan.FromSeconds(2.5 * 60),
				PowAllowMinDifficultyBlocks = false,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 1512,
				MinerConfirmationWindow = 2016,
				CoinbaseMaturity = 100,
				HashGenesisBlock = new uint256("4d96a915f49d40b1e5c2844d1ee2dccb90013a990ccea12c492d22110489f0c4"),
				GetPoWHash = GetPoWHash
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 71 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 5 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 128 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x88, 0xB2, 0x1E })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x88, 0xAD, 0xE4 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("vtc"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("vtc"))
			.SetMagic(0xdab5bffa)
			.SetPort(port)
			.SetRPCPort(5888)
			.SetName("vtc-main")
			.AddAlias("vtc-mainnet")
			.AddAlias("vertcoin-mainnet")
			.AddAlias("vertcoin-main")
			.AddDNSSeeds(new[]
			{
				new DNSSeedData("vtconline.org", "useast1.vtconline.org"),
				new DNSSeedData("gertjaap.org", "vtc.gertjaap.org"),
				new DNSSeedData("bryangoodson.org", "seed.vtc.bryangoodson.org"),
				new DNSSeedData("pknight.ca", "dnsseed.pknight.ca"),
				new DNSSeedData("orderofthetaco.org", "seed.orderofthetaco.org"),
				new DNSSeedData("alexturek.org", "seed.alexturek.org"),
				new DNSSeedData("mbl.cash", "vertcoin.mbl.cash")
			})
			.SetGenesis(new Block(Encoders.Hex.DecodeData("010000000000000000000000000000000000000000000000000000000000000000000000e72301fc49323ee151cf1048230f032ca589753ba7086222a5c023e3a08cf34a8b35cf52f0ff0f1e0eba57000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff460002e7034130312f30392f32303134204765726d616e7920746f2048656c7020696e20446973706f73616c206f662053797269616e204368656d6963616c20576561706f6e73ffffffff0100f2052a010000000000000000")))
			.BuildAndRegister();

			builder = new NetworkBuilder();
			port = 15889;
			_Testnet = builder.SetConsensus(new Consensus()
			{
				SubsidyHalvingInterval = 840000,
				MajorityEnforceBlockUpgrade = 51,
				MajorityRejectBlockOutdated = 75,
				MajorityWindow = 1000,
				PowLimit = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff")),
				PowTargetTimespan = TimeSpan.FromSeconds(3.5 * 24 * 60 * 60),
				PowTargetSpacing = TimeSpan.FromSeconds(2.5 * 60),
				PowAllowMinDifficultyBlocks = true,
				PowNoRetargeting = false,
				RuleChangeActivationThreshold = 1512,
				MinerConfirmationWindow = 2016,
				CoinbaseMaturity = 100,
				HashGenesisBlock = new uint256("cee8f24feb7a64c8f07916976aa4855decac79b6741a8ec2e32e2747497ad2c9"),
				GetPoWHash = GetPoWHash
			})
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, new byte[] { 74 })
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, new byte[] { 196 })
			.SetBase58Bytes(Base58Type.SECRET_KEY, new byte[] { 239 })
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, new byte[] { 0x04, 0x35, 0x87, 0xCF })
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, new byte[] { 0x04, 0x35, 0x83, 0x94 })
			.SetBech32(Bech32Type.WITNESS_PUBKEY_ADDRESS, Encoders.Bech32("tvtc"))
			.SetBech32(Bech32Type.WITNESS_SCRIPT_ADDRESS, Encoders.Bech32("tvtc"))
			.SetMagic(0x74726576)
			.SetPort(port)
			.SetRPCPort(15888)
			.SetName("vtc-test")
			.AddAlias("vtc-testnet")
			.AddAlias("vertcoin-test")
			.AddAlias("vertcoin-testnet")
			.AddSeeds(ToSeed(pnSeed6_test))
			.SetGenesis(new Block(Encoders.Hex.DecodeData("010000000000000000000000000000000000000000000000000000000000000000000000e72301fc49323ee151cf1048230f032ca589753ba7086222a5c023e3a08cf34af2b54a58f0ff0f1e53f60d000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff460002e7034130312f30392f32303134204765726d616e7920746f2048656c7020696e20446973706f73616c206f662053797269616e204368656d6963616c20576561706f6e73ffffffff0100f2052a010000000000000000")))
			.BuildAndRegister();


			var home = Environment.GetEnvironmentVariable("HOME");
			var localAppData = Environment.GetEnvironmentVariable("APPDATA");

			if(string.IsNullOrEmpty(home) && string.IsNullOrEmpty(localAppData))
				return new object();

			if(!string.IsNullOrEmpty(home))
			{
				var bitcoinFolder = Path.Combine(home, ".vertcoin");

				var mainnet = Path.Combine(bitcoinFolder, ".cookie");
				RPCClient.RegisterDefaultCookiePath(Networks._Mainnet, mainnet);

				var testnet = Path.Combine(bitcoinFolder, "testnet4", ".cookie");
				RPCClient.RegisterDefaultCookiePath(Networks._Testnet, testnet);

			}
			else if(!string.IsNullOrEmpty(localAppData))
			{
				var bitcoinFolder = Path.Combine(localAppData, "Vertcoin");

				var mainnet = Path.Combine(bitcoinFolder, ".cookie");
				RPCClient.RegisterDefaultCookiePath(Networks._Mainnet, mainnet);

				var testnet = Path.Combine(bitcoinFolder, "testnet4", ".cookie");
				RPCClient.RegisterDefaultCookiePath(Networks._Testnet, testnet);

	
			}
			return new object();
		}

		static uint256 GetPoWHash(BlockHeader header)
		{
			var headerBytes = header.ToBytes();
			var h = NBitcoin.Crypto.SCrypt.ComputeDerivedKey(headerBytes, headerBytes, 1024, 1, 1, null, 32);
			return new uint256(h);
		}

		private static IEnumerable<NetworkAddress> ToSeed(Tuple<byte[], int>[] tuples)
		{
			return tuples
					.Select(t => new NetworkAddress(new IPAddress(t.Item1), t.Item2))
					.ToArray();
		}

		private static Network _Mainnet;
		public static Network Mainnet
		{
			get
			{
				EnsureRegistered();
				return _Mainnet;
			}
		}

		private static Network _Testnet;
		public static Network Testnet
		{
			get
			{
				EnsureRegistered();
				return _Testnet;
			}
		}
	}
}
