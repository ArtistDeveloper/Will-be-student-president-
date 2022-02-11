using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class StatusClickAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Text statValue;
    public UnityEvent statClickEvent;
    public UnityEvent statClickCancleEvent;

    private Image transparentImg;
    private Color transparentImgColor;
    private bool isPressed;
    private Coroutine statValueAnim;
    private Coroutine statIconAnim;
    private Coroutine statTextAnim;
    private const float animSpeed = 50f;

    private void Start()
    {
        transparentImg = GetComponent<Image>();
        transparentImgColor = transparentImg.color;
        StartCoroutine(CoCheckClickEvent());
    }

    // #if UNITY_EDITOR
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        statClickEvent.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        statClickCancleEvent.Invoke();
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
                        statIconAnim = null;
                    }
                    statValueAnim = StartCoroutine(CoHideStatIconAnim());

                    // Text애니메이션 재생
                    statValue.gameObject.SetActive(true);
                    statTextAnim = StartCoroutine(CoShowTextAnim());
                    
                }
            }
            else
            {
                // 아이콘이 눌러지고 있지 않고, CoShowStatValue가 재생되고 있을 때.
                if (statValueAnim != null)
                {
                    StopCoroutine(statValueAnim);
                    StopCoroutine(statTextAnim);
                    statValueAnim = null;
                    statIconAnim = StartCoroutine(CoShowStatIconAnim());

                    statTextAnim = StartCoroutine(CoHideTextAnim());
                }
            }

            yield return null; // while에서 빠져나올 수 있도록 작업제어권 반환
        }
    }


    // ANCHOR : CoHideStatIconAnim()
    /// <summary>
    /// 아이콘을 클릭했을 때 Status아이콘을 숨기는 애니메이션을 재생합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoHideStatIconAnim()
    {
        Color changeColor = transparentImgColor;

        // lerp값 조절용
        float elapsedTime = 0;
        float progress = 1;

        while (changeColor.a >= 0.0f)
        {
            changeColor.a = Mathf.Lerp(0.0f, changeColor.a, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = progress - elapsedTime / animSpeed;

            transparentImg.color = changeColor;

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


    // ANCHOR : CoShowStatIconAnim()
    /// <summary>
    /// 아이콘에서 클릭을 멈췄을 때, 다시 아이콘을 보여주는 애니메이션을 재생합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoShowStatIconAnim()
    {
        Color changeColor = transparentImg.color;

        // lerp값 조절용
        float elapsedTime = 0;
        float progress = 0;

        while (changeColor.a < transparentImgColor.a)
        {
            changeColor.a = Mathf.Lerp(changeColor.a, transparentImgColor.a, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / animSpeed;

            transparentImg.color = changeColor;

            yield return null;
        }

        statIconAnim = null;
        yield break;
    }


    // ANCHOR : CoShowTextAnim()
    /// <summary>
    /// Text가 나타나는 애니메이션을 재생합니다.
    /// </summary>
    /// <returns></returns>
    /// // TODO : Text의 알파값을 0으로 하고 밝아지도록 만들어야 함. 유니티 엔진에서 건드려주면 어지러우니까, 코드에서 알파값 0으로 만들고 올라가도록 작업하기
    private IEnumerator CoShowTextAnim()
    {
        // Text alpha값이 0부터 밝아지도록 하기 위함
        Color changeTextColor = statValue.color;
        changeTextColor.a = 0;

        statValue.color = changeTextColor;

        float goalAlphaVal = 1;
        float elapsedTime = 0;
        float progress = 0;

        while (changeTextColor.a <= goalAlphaVal)
        {
            changeTextColor.a = Mathf.Lerp(0.0f, goalAlphaVal, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / 0.5f;

            statValue.color = changeTextColor;

            yield return null;
        }

        yield return null;
    }


    // ANCHOR : CoHideTextAnim()
    /// <summary>
    /// Text가 사라지는 애니메이션을 재생합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoHideTextAnim()
    {
        Color changeTextColor = statValue.color;

        float goalAlphaVal = statValue.color.a;
        float elapsedTime = 0;
        float progress = 1;

        while (changeTextColor.a > 0.0f) // >= 사용하면 while무한반복함.
        {
            changeTextColor.a = Mathf.Lerp(0.0f, changeTextColor.a, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = progress - elapsedTime / animSpeed;

            statValue.color = changeTextColor;

            yield return null;
        }

        yield return null;
    }
}

