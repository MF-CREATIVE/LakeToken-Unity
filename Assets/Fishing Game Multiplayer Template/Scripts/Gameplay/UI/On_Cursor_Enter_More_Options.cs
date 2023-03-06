using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class On_Cursor_Enter_More_Options : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent On_Enter;
    public UnityEvent On_Exit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        On_Enter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        On_Exit.Invoke();
    }
}
