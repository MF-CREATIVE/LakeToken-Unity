using System.Collections.Generic;
using UnityEngine;
using Thirdweb;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum Wallet
{
    MetaMask,
    Injected,
    CoinbaseWallet,
    WalletConnect,
    MagicAuth,
}

[Serializable]
public struct WalletButton
{
    public Wallet wallet;
    public GameObject walletButton;
    public Sprite icon;
}

[Serializable]
public struct NetworkSprite
{
    public Chain chain;
    public Sprite sprite;
}

public class Prefab_ConnectWallet : MonoBehaviour
{
    [Header("SETTINGS")]
    public List<Wallet> supportedWallets;
    public bool supportSwitchingNetwork;

    [Header("CUSTOM CALLBACKS")]
    public UnityEvent OnConnectedCallback;
    public UnityEvent OnDisconnectedCallback;
    public UnityEvent OnSwitchNetworkCallback;

    [Header("UI ELEMENTS (DO NOT EDIT)")]
    // Connecting
    public GameObject connectButton;
    public GameObject connectDropdown;
    public List<WalletButton> walletButtons;
    // Connected
    public GameObject connectedButton;
    public GameObject connectedDropdown;
    public TMP_Text balanceText;
    public TMP_Text walletAddressText;
    public Image walletImage;
    public TMP_Text currentNetworkText;
    public Image currentNetworkImage;
    public Image chainImage;
    // Network Switching
    public GameObject networkSwitchButton;
    public GameObject networkDropdown;
    public GameObject networkButtonPrefab;
    public List<NetworkSprite> networkSprites;

    string address;
    Wallet wallet;


    // UI Initialization
    public GameObject connectedState;
    public GameObject disconnectedState;
    public static int selectedBait;
    private Contract contractCollection;
    private Contract contractMarketplace;
    private Contract contractEditionDrop;
    public GameObject bait0;
    public GameObject bait1;
    public GameObject bait2;
    public GameObject bait3;
    private List<NFT> owned;

    private void Start()
    {
        address = null;

        if (supportedWallets.Count == 1)
            connectButton.GetComponent<Button>().onClick.AddListener(() => OnConnect(supportedWallets[0]));
        else
            connectButton.GetComponent<Button>().onClick.AddListener(() => OnClickDropdown());


        foreach (WalletButton wb in walletButtons)
        {
            if (supportedWallets.Contains(wb.wallet))
            {
                wb.walletButton.SetActive(true);
                wb.walletButton.GetComponent<Button>().onClick.AddListener(() => OnConnect(wb.wallet));
            }
            else
            {
                wb.walletButton.SetActive(false);
            }
        }

        connectButton.SetActive(true);
        connectedButton.SetActive(false);

        connectDropdown.SetActive(false);
        connectedDropdown.SetActive(false);

        networkSwitchButton.SetActive(supportSwitchingNetwork);
        networkDropdown.SetActive(false);
    }

    // Connecting

    public async void OnConnect(Wallet _wallet)
    {
        try
        {
            address = await ThirdwebManager.Instance.SDK.wallet.Connect(
               new WalletConnection()
               {
                   provider = GetWalletProvider(_wallet),
                   chainId = (int)ThirdwebManager.Instance.chain,
               });

            wallet = _wallet;
            OnConnected();
            if (OnConnectedCallback != null)
                OnConnectedCallback.Invoke();
            print($"Connected successfully to: {address}");

            ShowConnectedState();
            GetAllContracts();
            ShowMarketPlace();
        }
        catch (Exception e)
        {
            print($"Error Connecting Wallet: {e.Message}");
        }
    }

    async void OnConnected()
    {
        try
        {
            Chain _chain = ThirdwebManager.Instance.chain;
            CurrencyValue nativeBalance = await ThirdwebManager.Instance.SDK.wallet.GetBalance();
            balanceText.text = $"{nativeBalance.value.ToEth()} {nativeBalance.symbol}";
            walletAddressText.text = address.ShortenAddress();
            currentNetworkText.text = ThirdwebManager.Instance.chainIdentifiers[_chain];
            currentNetworkImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
            connectButton.SetActive(false);
            connectedButton.SetActive(true);
            connectDropdown.SetActive(false);
            connectedDropdown.SetActive(false);
            networkDropdown.SetActive(false);
            walletImage.sprite = walletButtons.Find(x => x.wallet == wallet).icon;
            chainImage.sprite = networkSprites.Find(x => x.chain == _chain).sprite;
        }
        catch (Exception e)
        {
            print($"Error Fetching Native Balance: {e.Message}");
        }

    }

    private void ShowConnectedState()
    {
        disconnectedState.SetActive(false);
        connectedState.SetActive(true);
    }

    // Disconnecting

    public async void OnDisconnect()
    {
        try
        {
            await ThirdwebManager.Instance.SDK.wallet.Disconnect();
            OnDisconnected();
            if (OnDisconnectedCallback != null)
                OnDisconnectedCallback.Invoke();
            print($"Disconnected successfully.");

        }
        catch (Exception e)
        {
            print($"Error Disconnecting Wallet: {e.Message}");
        }
    }

    void OnDisconnected()
    {
        address = null;
        connectButton.SetActive(true);
        connectedButton.SetActive(false);
        connectDropdown.SetActive(false);
        connectedDropdown.SetActive(false);

        DisconnectedState();
    }

    private void DisconnectedState()
    {
        connectedState.SetActive(false);
        disconnectedState.SetActive(true);
    }

    private void GetAllContracts()
    {
        // Get the NFT collection contract
        contractCollection = ThirdwebManager.Instance.SDK.GetContract("0x2eD61F881870268b4049007E4EbD49461ACe8558");

        // Get the Marketplace contract
        contractMarketplace = ThirdwebManager.Instance.SDK.GetContract("0xaa545c6b2dd42a59056d17B5D7382e55e9b23216");

        // Get the Edition Drop
        contractEditionDrop = ThirdwebManager.Instance.SDK.GetContract("0x2cD6d09a9c8f09821BB7188bf008DF529afB2D7E");
    }

    private async void ShowMarketPlace()
    {
        // First, check to see if the you own the NFT
        owned = await contractCollection.ERC1155.GetOwned();

        //Check to see if you own an NFT from the collection
        //CheckIfOwned(bait0, owned, 6, "3");
        CheckIfOwned(bait1, owned, 0, "0");
        CheckIfOwned(bait2, owned, 1, "1");
        CheckIfOwned(bait3, owned, 2, "2");
    }

    public void ChangeScene()
    {
        selectedBait = 0;
        SceneManager.LoadSceneAsync("Jays_Level");
    }

    // Check if player already owns any of the NFTs
    public async void CheckIfOwned(GameObject obj, List<NFT> NftOwned, int st, string token)
    {
        // if ownedNFTs contains a token with the same ID as the listing, then you own it
        //bool ownNFT = NftOwned.Exists(nft => nft.metadata.id == st.ToString());
        bool ownNFT = NftOwned.Exists(nft => nft.metadata.id == token);

        if (ownNFT)
        {
            // Apply the condition for owning the NFT
            var text1 = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            text1.text = "Play the game";

            //Set the on click to start the game by loading mane scene
            obj.GetComponent<UnityEngine.UI.Button>().onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            obj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                selectedBait = st;
                SceneManager.LoadSceneAsync("Jays_Level");
            });
        }
        else
        {
            // Once we have the price, we update the text to the price
            var price = await contractMarketplace.marketplace.GetListing(st.ToString());

            // Set the price in the button to buy
            var text1 = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            text1.text = "Buy:" + " " + price.buyoutCurrencyValuePerToken.displayValue + " " + price.buyoutCurrencyValuePerToken.symbol;
        }
    }

    public async void BuyNFT(string tokenId)
    {
        Debug.Log("Buy button clicked");

        Marketplace marketplace = contractMarketplace.marketplace;

        try
        {
            TransactionResult transactionResult = await marketplace.BuyListing(tokenId, 1);
            Debug.Log("Success! You have a new NFT" );
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        /*
        // claim
        var canClaim = await contractEditionDrop.ERC1155.claimConditions.CanClaim(tokenId, 1);
        if (canClaim)
        {
            try
            {
                var result = await contractEditionDrop.ERC1155.Claim(tokenId, 1);
                var newSupply = await contractEditionDrop.ERC1155.TotalSupply(tokenId);
                Debug.Log("Claim successful! New supply: " + newSupply);
            }
            catch (System.Exception e)
            {
                //fluxxText.GetComponent<TMPro.TextMeshProUGUI>().text = "Claim Failed: " + e.Message;
            }
        }
        else
        {
            Debug.Log("Can't claim at the moment");
            //fluxxText.GetComponent<TMPro.TextMeshProUGUI>().text = "Can't claim";
        }
        */
    }

    // Switching Network
    public async void OnSwitchNetwork(Chain _chain)
    {

        try
        {
            ThirdwebManager.Instance.chain = _chain;
            await ThirdwebManager.Instance.SDK.wallet.SwitchNetwork((int)_chain);
            OnConnected();
            if (OnSwitchNetworkCallback != null)
                OnSwitchNetworkCallback.Invoke();
            print($"Switched Network Successfully: {_chain}");

        }
        catch (Exception e)
        {
            print($"Error Switching Network: {e.Message}");
        }
    }

    // UI
    public void OnClickDropdown()
    {
        if (String.IsNullOrEmpty(address))
            connectDropdown.SetActive(!connectDropdown.activeInHierarchy);
        else
            connectedDropdown.SetActive(!connectedDropdown.activeInHierarchy);
    }

    public void OnClickNetworkSwitch()
    {
        if (networkDropdown.activeInHierarchy)
        {
            networkDropdown.SetActive(false);
            return;
        }

        networkDropdown.SetActive(true);

        foreach (Transform child in networkDropdown.transform)
            Destroy(child.gameObject);

        foreach (Chain chain in Enum.GetValues(typeof(Chain)))
        {
            if (chain == ThirdwebManager.Instance.chain || !ThirdwebManager.Instance.supportedNetworks.Contains(chain))
                continue;

            GameObject networkButton = Instantiate(networkButtonPrefab, networkDropdown.transform);
            networkButton.GetComponent<Button>().onClick.RemoveAllListeners();
            networkButton.GetComponent<Button>().onClick.AddListener(() => OnSwitchNetwork(chain));
            networkButton.transform.Find("Text_Network").GetComponent<TMP_Text>().text = ThirdwebManager.Instance.chainIdentifiers[chain];
            networkButton.transform.Find("Icon_Network").GetComponent<Image>().sprite = networkSprites.Find(x => x.chain == chain).sprite;
        }
    }

    // Utility
    WalletProvider GetWalletProvider(Wallet _wallet)
    {
        switch (_wallet)
        {
            case Wallet.MetaMask:
                return WalletProvider.MetaMask;
            case Wallet.Injected:
                return WalletProvider.Injected;
            case Wallet.CoinbaseWallet:
                return WalletProvider.CoinbaseWallet;
            case Wallet.WalletConnect:
                return WalletProvider.WalletConnect;
            case Wallet.MagicAuth:
                return WalletProvider.MagicAuth;
            default:
                throw new UnityException($"Wallet Provider for wallet {_wallet} unimplemented!");
        }
    }
}