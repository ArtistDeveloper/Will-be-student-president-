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
        Debug.Log("호출됐는데 왜 지랄이양");
        foreach (char letter in sentence.ToCharArray())
        {
            Debug.Log("호출됐는데 왜 지랄이양2");
            printDialogue += letter;
            dialgueText.text = printDialogue;
            yield return null;
        }
    }
}
