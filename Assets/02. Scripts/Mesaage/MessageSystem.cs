using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSystem : MonoBehaviour
{
    private Text dialgueText;
    private string printDialogue = "";
    private string testText = "엄청나게 긴 텍스트 ㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁ";

    private void Start()
    {
        dialgueText = GetComponent<Text>();
        dialgueText.text = printDialogue;

        // StopAllCoroutines();
        StartCoroutine(TypeSentence(testText));
    }

    IEnumerator TypeSentence(string sentence)
    {
        WaitForSeconds waitForSecond = new WaitForSeconds(0.01f); //여기 코드에서 미리 생성해놓고

        foreach (char letter in sentence.ToCharArray())
        {
            printDialogue += letter;
            dialgueText.text = printDialogue;
            yield return waitForSecond;
            // yield return null; 
        }
    }
}
