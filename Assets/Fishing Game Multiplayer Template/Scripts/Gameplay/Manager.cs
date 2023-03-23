using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Manager : MonoBehaviour
{
    public GameObject Menu;

    [Header("Fish Spawner Settings")]
    public FishSpawnerTest FishSpawner;
    [Header("Network Manager Settings")]
    public Mirror.NetworkManagerHUD NetworkManagerUI;
    public Toggle NetworkManagerToggle;
    [Header("Terrain Settings")]
    public Terrain SceneTerrain;
    public Toggle TerrainGrassToggle;
    [Header("Network")]
    public GameObject CreateServerMenu;
    public Mirror.NetworkManager NetworkManager;
    public InputField NetworkAddressInputField;
    [Header("Change Name")]
    public string PlayerName;
    public InputField PlayerNameInputField;

    public GameObject BuildGameInfo;

    public GameObject InGameChatSystem;
    private void Start()
    {
        ToggleNetworkManager();

        PlayerNameInputField.text += "Player ";

        const string numbers = "1234567890";

        int charAmount = Random.Range(0, 7);
        for (int i = 0; i < charAmount; i++)
        {
            PlayerNameInputField.text += numbers[Random.Range(0, numbers.Length)];
        }

        PlayerName = PlayerNameInputField.text;
    }

    public void HostServer()
    {
        NetworkManager.StartHost();
        Destroy(CreateServerMenu);

#if UNITY_EDITOR
        BuildGameInfo.SetActive(true);
#endif
    }

    public void EnterServer()
    {
        NetworkManager.networkAddress = NetworkAddressInputField.text;
        NetworkManager.StartClient();
        Destroy(CreateServerMenu);
    }

    void Update()
    {
        // Check if the in-game chat system is null and the local connection is not null
        if (InGameChatSystem == null && NetworkServer.localConnection != null)
        {
            // Get the player's game object
            GameObject playerObject = NetworkServer.localConnection?.identity?.gameObject;

            // If the player object exists, try to get the Inventory component
            if (playerObject != null)
            {
                Inventory inv = playerObject.GetComponent<Inventory>();

                // If the Inventory component exists, try to get the InWorldChatSystem game object
                if (inv != null && inv.InWorldChatSystem != null)
                {
                    GameObject inWorldChatSystemObject = inv.InWorldChatSystem.gameObject;

                    // If the InWorldChatSystem game object exists, toggle the menu based on user input
                    if (inWorldChatSystemObject != null)
                    {
                        if (Input.GetKeyDown(KeyCode.T) && !inWorldChatSystemObject.activeInHierarchy)
                        {
                            Menu.SetActive(true);
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.None;
                        }

                        if (Input.GetKeyUp(KeyCode.T) && inWorldChatSystemObject.activeInHierarchy)
                        {
                            Menu.SetActive(false);
                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Locked;
                        }
                    }
                }
            }
        }
    }

    public void SelectShiner()
    {
        FishSpawner._fishUniqueId = 0;
    }

    public void SelectTrout()
    {
        FishSpawner._fishUniqueId = 1;
    }

    public void ToggleNetworkManager()
    {
        if(NetworkManagerToggle.isOn == false)
        {
            //Hiding the mirror Network Manager GUI.
            if (NetworkManagerUI != null)
            {
                NetworkManagerUI.showGUI = false;
            }
        }

        if (NetworkManagerToggle.isOn == true)
        {
            //Activate the mirror Network Manager GUI.
            if (NetworkManagerUI != null)
            {
                NetworkManagerUI.showGUI = true;
            }
        }
    }

    public void ToggleTerrainGrass()
    {
        if (TerrainGrassToggle.isOn == false)
        {
            SceneTerrain.drawTreesAndFoliage = false;
        }

        if (TerrainGrassToggle.isOn == true)
        {
            SceneTerrain.drawTreesAndFoliage = true;
        }
    }
}
