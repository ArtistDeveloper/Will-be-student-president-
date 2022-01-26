using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image statusValueBackground;

    private bool isPressed;

    private void Start()
    {
        StartCoroutine(CheckClickEvent());
    }

    // #if UNITY_EDITOR
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
    // #endif


    private IEnumerator CheckClickEvent()
    {
        while (true)
        {
            if (isPressed)
            {
                Debug.Log("눌러지고있어요");
            }
            else
            {
                Debug.Log("안눌러지고있어요");
                // yield return null;
            }
            yield return null;
        }
    }

    public void ShowStatusValue()
    {

    }
}
