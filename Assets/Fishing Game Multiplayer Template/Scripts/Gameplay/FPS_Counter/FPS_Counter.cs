using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{

    public float timer, refresh, avgFramerate;
    string display = "{0} FPS";
    public Text m_Text;
    public GameObject FPS_Canvas;
    public bool isFPSEnabled;

    private void Start()
    {
        isFPSEnabled = false;
        FPS_Canvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if(isFPSEnabled == false)
            {
                FPS_Canvas.SetActive(true);
                isFPSEnabled = true;
            }
            else
            {
                isFPSEnabled = false;
                FPS_Canvas.SetActive(false);
            }
        }

        if (isFPSEnabled == true)
        {
            float timelapse = Time.smoothDeltaTime;
            timer = timer <= 0 ? refresh : timer -= timelapse;

            if (timer <= 0) avgFramerate = (int)(1f / timelapse);
            m_Text.text = string.Format(display, avgFramerate.ToString());
        }
    }
}
