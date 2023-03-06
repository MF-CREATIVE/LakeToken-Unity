using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitActivator : MonoBehaviour
{
    public Baits[] Baits;
}

[System.Serializable]
public class Baits
{
    public GameObject Bait;
    public int ID;

    public void EnableBait()
    {
        Bait.SetActive(true);
    }
}
