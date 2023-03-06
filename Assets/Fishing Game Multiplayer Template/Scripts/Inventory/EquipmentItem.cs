using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentItem : MonoBehaviour
{
    public Inventory InventorySystem;
    [Header("Equipment Item")]
    public Text EquipmentName;
    public Sprite EquipmentImage;
    public int EquipmentID;

    //Types: Fishing Rod, Float, Bait
    public string EquipmentType;

    private static FishingFloatScriptable[] _floatScriptables;
    private FishingFloatScriptable _scriptable;

    private void Awake()
    {
        if (_floatScriptables == null)
        {
            _floatScriptables = Resources.LoadAll<FishingFloatScriptable>("FishingFloats");
        }

        if (EquipmentType == "Fishing Rod")
        {

        }

        if (EquipmentType == "Float")
        {
            for (int i = 0; i < _floatScriptables.Length; i++)
            {
                if (_floatScriptables[i].uniqueId == EquipmentID)
                {
                    _scriptable = _floatScriptables[i];
                    break;
                }
            }
        }

        if (EquipmentType == "Bait")
        {

        }
    }

    public void SelectEquipment()
    {
        if(EquipmentType == "Fishing Rod")
        {
            InventorySystem.CurrentSelectedFishingRod = EquipmentID;
            InventorySystem.CurrentSelectedFishingRodImage.sprite = EquipmentImage;
            InventorySystem.CurrentSelectedFishingRodImage.color = Color.white;
            InventorySystem.SetFishingRod(EquipmentID);
            InventorySystem.FishingRodSelectionMenu.SetActive(false);
        }

        if (EquipmentType == "Float")
        {
            InventorySystem.LastSelectedFloat = InventorySystem.CurrentSelectedFloat;
            InventorySystem.CurrentSelectedFloat = EquipmentID;
            InventorySystem.SetFloat(EquipmentID);
            InventorySystem.CurrentSelectedFloatImage.sprite = EquipmentImage;
            InventorySystem.CurrentSelectedFloatImage.color = Color.white;
            InventorySystem.SetUpFloat();
            InventorySystem.FloatSelectionMenu.SetActive(false);
            InventorySystem.FloatHasChanged = true;
        }

        if (EquipmentType == "Bait")
        {
            InventorySystem.CurrentSelectedBait = EquipmentID;
            InventorySystem.SetBait(EquipmentID);
            InventorySystem.CurrentSelectedBaitImage.sprite = EquipmentImage;
            InventorySystem.CurrentSelectedBaitImage.color = Color.white;
            InventorySystem.SetUpFloat();
            InventorySystem.BaitSelectionMenu.SetActive(false);
        }
    }

    private void Update()
    {
        //Force size to 0.555277
        if (this.GetComponent<RectTransform>().localScale.x != 0.555277)
        {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.555277f, 0.555277f, 0.555277f);
        }
    }
}
