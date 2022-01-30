using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Text statValue;

    private Image originImg;
    private Color originImgColor;
    private bool isPressed;
    private Coroutine statValueAnim;
    private Coroutine statIconAnim;

    private void Start()
    {
        originImg = GetComponent<Image>();
        originImgColor = originImg.color;
        StartCoroutine(CoCheckClickEvent());
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


    private IEnumerator CoCheckClickEvent()
    {
        while (true)
        {
            if (isPressed)
            {
                // 아이콘이 눌러지고 있고, CoShowStatValue가 실행되고 있지 않는 상태
                if (statValueAnim == null)
                {
                    if (statIconAnim != null)
                    {
                        StopCoroutine(statIconAnim);
                        RevertStatIcon();
                        statIconAnim = null;
                    }

                    statValueAnim = StartCoroutine(CoShowStatValueAnim());
                }
            }
            else
            {
                // 아이콘이 눌러지고 있지 않고, CoShowStatValue가 재생되고 있을 때.
                if (statValueAnim != null)
                {
                    StopCoroutine(statValueAnim);
                    statValueAnim = null;
                    statIconAnim = StartCoroutine(CoShowStatIconAnim());
                }
            }

            yield return null; // while에서 빠져나올 수 있도록 작업제어권 반환
        }
    }



    // TODO : statvalue값을 들고와서 텍스트 값을 보여주도록 만들어야 한다.
    /// <summary>
    /// 아이콘을 클릭했을 때 Status의 값을 숫자로 보여주는 함수입니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoShowStatValueAnim()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        Color changeColor = originImgColor;

        float elapsedTime = 0;
        float progress = 1;

        Debug.Log($"changeColor.a : {changeColor.a}");

        while (changeColor.a >= 0.0f)
        {
            changeColor.a = Mathf.Lerp(0.0f, changeColor.a, progress);
            Debug.Log(changeColor.a);

            elapsedTime += Time.unscaledDeltaTime;
            progress = progress - elapsedTime / 50; // 프레임사이의 간격의 값을 더 크게 나눠서 뺴면 속도가 줄어들겠지. 예를들어 값이 5정도면 더 아이콘이 빨리없어질것이다.

            originImg.color = changeColor;

            yield return null;
        }

        // 예외처리 : 마우스클릭을 끝까지했을 떄, yield break를 만나서 밝아지고 다시 어두워지는 것의 대한 예외처리
        while (true)
        {
            if (!isPressed)
            {
                statValueAnim = null;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 아이콘에서 클릭을 멈췄을 때, 다시 아이콘을 보여주는 함수입니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoShowStatIconAnim()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        Color changeColor = originImg.color;

        while (changeColor.a < originImgColor.a)
        {
            changeColor.a += 0.1f;
            originImg.color = changeColor;

            yield return waitForSeconds;
        }

        statIconAnim = null;
        yield break;
    }

    // TODO : 개발하자
    // 아이콘의 알파값을 원래대로 되돌리는 역할
    private void RevertStatIcon()
    {

    }
}
