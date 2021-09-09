using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystem : MonoBehaviour
{
    // 가비지 생기니 string buffer 사용해볼까. 일단 생산성이 중요하니 DoTween에셋을 써보자
    string testText = "엄청나게 긴 텍스트 ㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁ";

    private void Start()
    {
        Text text = GetComponent<Text>();
        text.text = testText;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
