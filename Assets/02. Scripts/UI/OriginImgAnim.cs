using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginImgAnim : MonoBehaviour
{

    [HideInInspector] public int stat;

    private Image originImg;
    private Coroutine originImgCoroutine;
    private const int statMax = 100;


    private void Start()
    {
        originImg = GetComponent<Image>();

        // 이미지 채우기 용 셋업
        originImg = GetComponent<Image>();
        originImg.fillAmount = 0f;
        originImg.type = Image.Type.Filled;
        originImg.fillMethod = Image.FillMethod.Vertical;
        originImg.fillOrigin = (int)Image.OriginVertical.Bottom;
    }


    private void Update()
    {
        // Debug.Log(Time.time);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(CoChargeStatAnim(30f, 70f));
        }
    }


    public void CallCoHideOriginImg()
    {
        if (originImgCoroutine != null)
        {
            StopCoroutine(originImgCoroutine);
            originImgCoroutine = null;
        }
        originImgCoroutine = StartCoroutine(CoHideOriginImg());
    }


    public void CallCoShowOriginImg()
    {
        if (originImgCoroutine != null)
        {
            StopCoroutine(originImgCoroutine);
            originImgCoroutine = null;
        }
        originImgCoroutine = StartCoroutine(CoShowOriginImg());
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoHideOriginImg()
    {
        Color changeColor = originImg.color;

        // lerp값 조절용
        float elapsedTime = 0;
        float progress = 1;

        while (changeColor.a >= 0.0f)
        {
            changeColor.a = Mathf.Lerp(0.0f, changeColor.a, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = progress - elapsedTime / 5;

            originImg.color = changeColor;

            yield return null;
        }

        yield return null;
    }


    // FIXME : 최적화 필요
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoShowOriginImg()
    {
        Color changeColor = originImg.color;
        Debug.Log($"OriginImgColor의 a값 : {originImg.color.a}");

        // lerp값 조절용
        float elapsedTime = 0;
        float progress = 0;

        while (changeColor.a < 255)
        {
            Debug.Log($"changeColor.a : {changeColor.a}");
            Debug.Log($"progress : {progress}");

            changeColor.a = Mathf.Lerp(changeColor.a, 255f, progress);

            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / 50;

            originImg.color = changeColor;
            Debug.Log($"originImg.color : {originImg.color}");

            yield return null;
        }

        yield return null;
    }


    // FIXME : while문 최적화 필요
    /// <summary>
    /// 스탯의 애니메이션을 재생하는 함수입니다.
    /// </summary>
    /// <param name="startVal"></param> 값
    /// <param name="goalVal"></param>
    /// <returns></returns>
    private IEnumerator CoChargeStatAnim(float startVal, float goalVal)
    {
        float changeVal = startVal;
        float elapsedTime = 0;
        float progress = 0;

        // 이걸로 몇초가 걸리는진 알 수 없는게, progress에 따라 보간값이 바뀌는 것이기 때문.
        while (changeVal <= goalVal)
        {
            // changeVal = Mathf.Lerp((float)changeVal, (float)goalVal, Time.smoothDeltaTime / 2.5f);
            changeVal = Mathf.Lerp((float)changeVal, (float)goalVal, progress);
            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / 100f;

            // Debug.Log("changeVal : " + changeVal);

            originImg.fillAmount = changeVal / 100f;
            // Debug.Log("originImg.fillAmount : " + originImg.fillAmount);

            yield return null;
        }
    }
}
