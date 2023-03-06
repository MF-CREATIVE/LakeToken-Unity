using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnCursorEnter_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public Animator anim;
    //public string Fadein_Anim_Name;
    //public string Fadeout_Anim_Name;
    public GameObject PopUp_Window;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //anim.Play(Fadein_Anim_Name);
        PopUp_Window.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //anim.Play(Fadeout_Anim_Name);
        PopUp_Window.SetActive(false);
    }
}
