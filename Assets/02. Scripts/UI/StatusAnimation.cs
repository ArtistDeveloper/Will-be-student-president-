using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusAnimation : MonoBehaviour
{
    public Image img;
    [HideInInspector] public int stat;

    private const int statMax = 100;

    private void Start()
    {
        img.fillAmount = 0f;
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Vertical;
        img.fillOrigin = (int)Image.OriginVertical.Bottom;
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Tab))
    //     {
    //         StartCoroutine(PlayStatAnim(30f, 70f));
    //     }
    // }


    /// <summary>
    /// 스탯의 애니메이션을 재생하는 함수입니다.
    /// </summary>
    /// <param name="startVal"></param> 값
    /// <param name="goalVal"></param>
    /// <returns></returns>
    private IEnumerator PlayStatAnim(float startVal, float goalVal)
    {
        float changeVal = startVal;
        float elapsedTime = 0;
        float progress = 0;

        while (changeVal <= goalVal)
        {
            // changeVal = Mathf.Lerp((float)changeVal, (float)goalVal, Time.smoothDeltaTime / 2.5f);
            changeVal = Mathf.Lerp((float)changeVal, (float)goalVal, progress);
            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / 5f;

            Debug.Log("changeVal : " + changeVal);

            img.fillAmount = changeVal / 100f;
            Debug.Log("img.fillAmount : " + img.fillAmount);

            yield return null;
        }
    }
}
