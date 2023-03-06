using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Inventory : NetworkBehaviour
{
    [Header("Inventory")]
    public GameObject InventoryCanvas;
    public Transform Content;
    [Header("Items")]
    public GameObject InventoryFishPrefab;
    [Header("Messages")]
    public GameObject NoItemsInInventoryMessage;
    [Header("Player GameObjects")]
    public GameObject[] Fishes;
    public GameObject FishHolder;
    [Header("Animation")]
    public string FishHolderAnimationName = "Fish_Caught";
    [Header("Fishing Line")]
    [SerializeField] public Transform _rodEndPoint;
    [SerializeField] private LineRenderer _rodLineRenderer;
    public GameObject LineStart;
    public GameObject LineEnd;
    public GameObject LineEndPrefab;
    GameObject SpawnedLineEndPrefab;
    [SyncVar]
    public bool FloatHasChanged = false;
    [Header("Camera")]
    public Camera Camera;
    [Header("Equipment")]
    //Floats
    [SyncVar]
    public int CurrentSelectedFloat = 0;
    [SyncVar]
    public int LastSelectedFloat = 0;
    public Float[] Floats;
    public Transform FloatContent;
    public GameObject FloatSelectionMenu;
    public Image CurrentSelectedFloatImage;
    //Fishing Rod
    [SyncVar]
    public int CurrentSelectedFishingRod = 0;
    public FishingRod[] FishingRods;
    public Transform FishingRodContent;
    public GameObject FishingRodSelectionMenu;
    public Image CurrentSelectedFishingRodImage;
    //Bait
    [SyncVar]
    public int CurrentSelectedBait = 0;
    public Bait[] Baits;
    public Transform BaitContent;
    public GameObject BaitSelectionMenu;
    public Image CurrentSelectedBaitImage;
    [Header("Player Name")]
    public GameObject Manager;
    public string PlayerName;
    public Text PlayerNameText;

    private void Start()
    {
        Manager = GameObject.FindGameObjectWithTag("Manager");
        PlayerName = Manager.GetComponent<Manager>().PlayerName;

        SetPlayerName(PlayerName);

        SetUpFloat();

        if (!isLocalPlayer)
            return;

        CheckForItems();

        SpawnEquipment();
    }

    public void SetPlayerName(string PlayerN)
    {
        PlayerN = PlayerName;
        PlayerNameText.text = PlayerN;

        CmdSetPlayerName(PlayerN);
    }

    [Command]
    public void CmdSetPlayerName(string PlayerN)
    {
        PlayerN = PlayerName;
        PlayerNameText.text = PlayerN;

        RpcSetPlayerName(PlayerN);
    }

    [ClientRpc]
    public void RpcSetPlayerName(string PlayerN)
    {
        PlayerN = PlayerName;
        PlayerNameText.text = PlayerN;
    }

    public void SetUpFloat()
    {
        for (int i = 0; i < Floats.Length; i++)
        {
            if(Floats[i].ID == CurrentSelectedFloat)
            {
                SpawnedLineEndPrefab = Instantiate(Floats[i].LineEndPrefab, transform.position + (transform.forward * 2), Floats[i].LineEndPrefab.transform.rotation);
            }
        }

        if (LineEnd != null)
            Destroy(LineEnd);

        LineEnd = SpawnedLineEndPrefab;

        LineStart.GetComponent<SpringJoint>().connectedBody = SpawnedLineEndPrefab.GetComponent<Rigidbody>();

        for (int i = 0; i < SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits.Length; i++)
        {
            SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(false);

            if (SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].ID == CurrentSelectedBait)
            {
                SpawnedLineEndPrefab.GetComponent<BaitActivator>().Baits[i].Bait.SetActive(true);
            }
        }
    }

    public void SetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        CmdSetFloat(ID);
    }

    [Command]
    public void CmdSetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        SetUpFloat();

        RpcSetFloat(ID);
    }

    [ClientRpc]
    public void RpcSetFloat(int ID)
    {
        CurrentSelectedFloat = ID;

        SetUpFloat();
    }

    public void SetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;
        CmdSetFishingRod(ID);

        SetUpFloat();
    }

    [Command]
    public void CmdSetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].FishingRodGameObject.SetActive(false);

            if (FishingRods[i].ID == CurrentSelectedFishingRod)
            {
                FishingRods[i].FishingRodGameObject.SetActive(true);

                RpcSetFishingRod(ID);
            }
        }
    }

    [ClientRpc]
    public void RpcSetFishingRod(int ID)
    {
        CurrentSelectedFishingRod = ID;

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].FishingRodGameObject.SetActive(false);

            if (FishingRods[i].ID == CurrentSelectedFishingRod)
            {
                FishingRods[i].FishingRodGameObject.SetActive(true);
            }
        }
    }

    public void SetBait(int ID)
    {
        CurrentSelectedBait = ID;

        CmdSetBait(ID);
    }

    [Command]
    public void CmdSetBait(int ID)
    {
        CurrentSelectedBait = ID;

        SetUpFloat();

        RpcSetBait(ID);
    }

    [ClientRpc]
    public void RpcSetBait(int ID)
    {
        CurrentSelectedBait = ID;

        SetUpFloat();
    }

    public void SpawnEquipment()
    {
        for (int i = 0; i < Floats.Length; i++)
        {
            Floats[i].Spawn();
        }

        for (int i = 0; i < FishingRods.Length; i++)
        {
            FishingRods[i].Spawn();
        }

        for (int i = 0; i < Baits.Length; i++)
        {
            Baits[i].Spawn();
        }
    }

    public void SpawnFloatUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);
        SpawnedPrefab.GetComponent<EquipmentItem>().InventorySystem = this;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentType = "Float";
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentID = ID;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentName.text = Name;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentImage = Image;
    }

    public void SpawnFishingRodUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);
        SpawnedPrefab.GetComponent<EquipmentItem>().InventorySystem = this;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentType = "Fishing Rod";
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentID = ID;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentName.text = Name;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentImage = Image;
    }

    public void SpawnBaitUI(GameObject prefab, Transform parent, int ID, string Name, Sprite Image)
    {
        GameObject SpawnedPrefab;

        SpawnedPrefab = Instantiate(prefab);
        SpawnedPrefab.transform.SetParent(parent);
        SpawnedPrefab.GetComponent<EquipmentItem>().InventorySystem = this;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentType = "Bait";
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentID = ID;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentName.text = Name;
        SpawnedPrefab.GetComponent<EquipmentItem>().EquipmentImage = Image;
    }

    private void Update()
    {
        if (FloatHasChanged == true & LastSelectedFloat != CurrentSelectedFloat)
        {
            SetUpFloat();
            FloatHasChanged = false;
        }

        if (this.GetComponent<PlayerFishing>()._fishingFloat == null)
        {
            _rodLineRenderer.SetPosition(0, _rodEndPoint.position);
            _rodLineRenderer.SetPosition(1, LineEnd.transform.position);
            LineEnd.SetActive(true);
            LineStart.SetActive(true);
        }
        else
        {
            LineStart.SetActive(false);
            LineEnd.SetActive(false);
        }

        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            this.GetComponent<TestPlayerController>().canRotateCamera = false;
            InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeOut");
            InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeIn");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            this.GetComponent<TestPlayerController>().canRotateCamera = true;
            InventoryCanvas.GetComponent<Animator>().ResetTrigger("FadeIn");
            InventoryCanvas.GetComponent<Animator>().SetTrigger("FadeOut");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void AddFishItem(int uniqueId, string FishName, string FishLength, string FishWeight, string FishRetailValue, Sprite FishSprite)
    {
        /*if (!isLocalPlayer)
            return;*/

        GameObject SpawnedInventoryFish;

        this.GetComponent<Animator>().SetTrigger(FishHolderAnimationName);

        HoldCaughtFish(uniqueId);

        SpawnedInventoryFish = Instantiate(InventoryFishPrefab);
        SpawnedInventoryFish.transform.SetParent(Content);
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishName.text = FishName;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishLength.text = FishLength;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishWeight.text = "Weight: " + FishWeight;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishRetailValue.text = FishRetailValue;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishImage.sprite = FishSprite;

        CmdHoldCaughtFish(uniqueId);

        SpawnedInventoryFish = null;

        CheckForItems();
    }

    public void HoldCaughtFish(int uniqueId)
    {
        FishHolder.SetActive(true);
        Fishes[uniqueId].gameObject.SetActive(true);
    }

    [Command]
    public void CmdHoldCaughtFish(int uniqueId)
    {
        HoldCaughtFish(uniqueId);

        RpcHoldCaughtFish(uniqueId);
    }

    [ClientRpc]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        HoldCaughtFish(uniqueId);
    }

    public void CheckForItems()
    {
        if (!isLocalPlayer)
            return;

        if (Content.childCount < 1)
        {
            NoItemsInInventoryMessage.SetActive(true);
        }
        if (Content.childCount > 0)
        {
            NoItemsInInventoryMessage.SetActive(false);
        }
    }

    public void ToggleCameraClippingPlanes(float Value)
    {
        Camera.nearClipPlane = Value;
    }
}

[System.Serializable]
public class Float
{
    public Inventory InventorySystem;
    public GameObject FloatPrefab;
    public GameObject LineEndPrefab;
    [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnFloatUI(FloatPrefab, InventorySystem.FloatContent, ID, Name, Image);
    }
}

[System.Serializable]
public class FishingRod
{
    public Inventory InventorySystem;
    public GameObject FishingRodPrefab;
    public GameObject FishingRodGameObject;
    [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnFishingRodUI(FishingRodPrefab, InventorySystem.FishingRodContent, ID, Name, Image);
    }
}

[System.Serializable]
public class Bait
{
    public Inventory InventorySystem;
    public GameObject BaitPrefab;
    [SyncVar]
    public int ID;
    public string Name;
    public Sprite Image;

    public void Spawn()
    {
        InventorySystem.SpawnBaitUI(BaitPrefab, InventorySystem.BaitContent, ID, Name, Image);
    }
}

