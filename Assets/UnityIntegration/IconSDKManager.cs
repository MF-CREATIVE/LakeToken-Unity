
using UnityEngine;
using IconSDK.RPCs;
using System.Numerics;
using IconSDK;
using IconSDK.Account;
using IconSDK.Types;
using NBitcoin;
using NBitcoin.DataEncoders;
using Network = NBitcoin.Network;
using Cysharp.Threading.Tasks;

public class IconSDKManager : MonoBehaviour
{
    public string MainNetApiUri = "https://wallet.icon.foundation/api/v3";
    public string TestNetApiUri = "https://testwallet.icon.foundation/api/v3";

    public static IconSDKManager Instance;

    async void Start()
    {
        Instance = this;
    }

    public async UniTask<Hash32> GetBlockByHeight(BigInteger height)
    {
        var get = new GetBlockByHeight(Consts.ApiUrl.MainNet);
        var result = await get.Invoke(height);
        return result.Hash;
    }

    public async UniTask<Hash32> GetLastBlockAsync()
    {
        var get = new GetLastBlock(Consts.ApiUrl.MainNet);
        Debug.Log("hi hi hi ");

        var result = await get.Invoke();
        Debug.Log("hi hi hi hi");
        return result.Hash;
    }

    public async UniTask<BigInteger> GetTotalSupplyAsync()
    {
        var getTotalSupply = new GetTotalSupply(Consts.ApiUrl.MainNet);
        var totalSupply = await getTotalSupply.Invoke();
        return totalSupply;
    }

    public Wallet GetWalletFromMnemonic(string mnemonicString)
    {
        Mnemonic mnemonic = new Mnemonic(mnemonicString);

        byte[] bytes = GetPrivateKeyBytesFromMnemonic(mnemonic.ToString());

        Wallet wallet = Wallet.Create(new PrivateKey(bytes));

        return wallet;
    }

    public byte[] GetPrivateKeyBytesFromMnemonic(string mnemonicString)
    {
        Mnemonic mnemonic = new Mnemonic(mnemonicString);
        ExtKey extKey = mnemonic.DeriveExtKey();
        Key privKey = extKey.PrivateKey;
        return privKey.ToBytes();
    }

    public async UniTask<double> GetBalanceAsync(string address, NetworkType network)
    {
        GetBalance getBalance = new GetBalance(GetApiUri(network));
        BigInteger balance = await getBalance.Invoke(address);

        return balance.ToCoins(BigIntegerExtension.ICXDecimals);
    }

    private string GetApiUri(NetworkType network)
    {
        if (network == NetworkType.Mainnet)
            return MainNetApiUri;

        return TestNetApiUri;
    }
}

public enum NetworkType
{
    Mainnet, Testnet
}

public class MainnetNetwork : Network
{
    public MainnetNetwork()
    {
        this.Base58Prefixes = new byte[12][];
        this.Base58Prefixes[(int)Base58Type.PUBKEY_ADDRESS] = new byte[] { 127 }; // t
        this.Base58Prefixes[(int)Base58Type.SCRIPT_ADDRESS] = new byte[] { 137 }; // x
        this.Base58Prefixes[(int)Base58Type.SECRET_KEY] = new byte[] { (239) };
        this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_NO_EC] = new byte[] { 0x01, 0x42 };
        this.Base58Prefixes[(int)Base58Type.ENCRYPTED_SECRET_KEY_EC] = new byte[] { 0x01, 0x43 };
        this.Base58Prefixes[(int)Base58Type.EXT_PUBLIC_KEY] = new byte[] { (0x04), (0x35), (0x87), (0xCF) };
        this.Base58Prefixes[(int)Base58Type.EXT_SECRET_KEY] = new byte[] { (0x04), (0x35), (0x83), (0x94) };
        this.Base58Prefixes[(int)Base58Type.PASSPHRASE_CODE] = new byte[] { 0x2C, 0xE9, 0xB3, 0xE1, 0xFF, 0x39, 0xE2 };
        this.Base58Prefixes[(int)Base58Type.CONFIRMATION_CODE] = new byte[] { 0x64, 0x3B, 0xF6, 0xA8, 0x9A };
        this.Base58Prefixes[(int)Base58Type.STEALTH_ADDRESS] = new byte[] { 0x2b };
        this.Base58Prefixes[(int)Base58Type.ASSET_ID] = new byte[] { 115 };
        this.Base58Prefixes[(int)Base58Type.COLORED_ADDRESS] = new byte[] { 0x13 };

        Bech32Encoder encoder = Encoders.Bech32("tb");
        this.Bech32Encoders = new Bech32Encoder[2];
        this.Bech32Encoders[(int)Bech32Type.WITNESS_PUBKEY_ADDRESS] = encoder;
        this.Bech32Encoders[(int)Bech32Type.WITNESS_SCRIPT_ADDRESS] = encoder;
    }
}