using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishCaughtMessage : MonoBehaviour
{
    public Text Message;
    public string FadeOutAnimationName = "MessageOut";
    public float FadeOutTime = 5f;

    public void Start()
    {
        StartCoroutine(FadeOutMessage());
    }

    IEnumerator FadeOutMessage()
    {
        yield return new WaitForSeconds(FadeOutTime);

        this.GetComponent<Animator>().Play(FadeOutAnimationName);

        StartCoroutine(DestroyMessage());
    }

    IEnumerator DestroyMessage()
    {
        yield return new WaitForSeconds(1.2f);

        Destroy(this.gameObject);
    }
}
