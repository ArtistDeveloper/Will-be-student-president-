using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image originImg;

    private bool isPressed;
    private Coroutine statValueAnim;

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
                if (statValueAnim == null)
                {
                    statValueAnim = StartCoroutine(ShowStatusValue());
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                if (statValueAnim != null)
                {
                    StopCoroutine(statValueAnim);
                    statValueAnim = null;

                    // StartCoroutine(ShowStatusIcon());
                }
            }

            yield return null;
        }
    }

    private IEnumerator ShowStatusValue()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        Color color = originImg.color;

        while (originImg.color.a >= 0.0)
        {
            color.a -= 0.1f;
            originImg.color = color;

            yield return waitForSeconds;
        }

        yield return null;
    }

    private IEnumerator ShowStatusIcon()
    {
        yield return null;
    }
}
