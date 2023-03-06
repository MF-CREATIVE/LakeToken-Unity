using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatSimulation : MonoBehaviour
{
    [Header("Float Simulation")]
    public Animator Anim;
    public bool canPull = true;
    public bool isSimulatingBite = false;

    private void Update()
    {
        if(canPull == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Anim.SetTrigger("StartPullUp");
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Anim.SetTrigger("PullUpEnd");
            }
        }
    }

    public void SimulateBite()
    {
        if(isSimulatingBite == false)
        {
            canPull = false;
            Anim.SetTrigger("FishBite");
            isSimulatingBite = true;
        }
    }
}
