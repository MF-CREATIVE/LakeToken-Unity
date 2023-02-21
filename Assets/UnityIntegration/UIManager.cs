using System.Numerics;
using IconSDK;
using IconSDK.Account;
using IconSDK.Types;
using NBitcoin;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public InputField AddressInputField, MnemonicInputField, SendAddressInputField, SendAmountInputField, HeightInputField;

    public Button GetBalanceButton, GenerateMnemonicButton, InitWalletButton, CopyAddressButton, SendButton, GetTotalSupplyButton, GetLastBlockButton, GetBlockByHeightButton;

    public Text BalanceText, WalletAddressText, TotalSupplyText, LastBlockHashText, BlockByHeightText;

    private Wallet wallet = null;

    void Start()
    {
        GetBalanceButton.onClick.AddListener(async delegate
        {
            var balance = await IconSDKManager.Instance.GetBalanceAsync(AddressInputField.text, NetworkType.Mainnet);

            BalanceText.text = balance.ToString() + " ICX";
        });

        GenerateMnemonicButton.onClick.AddListener(async delegate
        {
            Mnemonic mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            MnemonicInputField.text = mnemonic.ToString();
        });

        InitWalletButton.onClick.AddListener(async delegate
        {
            wallet = IconSDKManager.Instance.GetWalletFromMnemonic(MnemonicInputField.text);

            WalletAddressText.text = wallet.Address.ToString();
        });

        CopyAddressButton.onClick.AddListener(async delegate
        {
            GUIUtility.systemCopyBuffer = WalletAddressText.text;
        });

        SendButton.onClick.AddListener(async delegate
        {
            string address = SendAddressInputField.text;
            BigInteger amount = Consts.ICX2Loop.MultiplyByDouble(double.Parse(SendAmountInputField.text));
            Hash32 result = await wallet.Transfer(address, amount, Consts.ICX2Loop, 1);

            Debug.Log(result);
        });

        GetTotalSupplyButton.onClick.AddListener(async delegate
        {
            TotalSupplyText.text = (await IconSDKManager.Instance.GetTotalSupplyAsync()).ToString();
        });

        GetLastBlockButton.onClick.AddListener(async delegate
        {
            LastBlockHashText.text = (await IconSDKManager.Instance.GetLastBlockAsync()).ToString();
        });

        GetBlockByHeightButton.onClick.AddListener(async delegate
        {
            BlockByHeightText.text = (await IconSDKManager.Instance.GetBlockByHeight(int.Parse(HeightInputField.text))).ToString();
        });
    }
}
