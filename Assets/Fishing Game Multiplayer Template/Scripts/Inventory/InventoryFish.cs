using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryFish : MonoBehaviour
{
    [Header("Fish")]
    public Text FishName;
    public Text FishLength;
    public Text FishWeight;
    public Text FishRetailValue;
    public Image FishImage;

    private void Update()
    {
        //Force size to 0.9434147
        if (this.GetComponent<RectTransform>().localScale.x != 0.9434147)
        {
            this.GetComponent<RectTransform>().localScale = new Vector3(0.9434147f, 0.9434147f, 0.9434147f);
        }
    }
}
