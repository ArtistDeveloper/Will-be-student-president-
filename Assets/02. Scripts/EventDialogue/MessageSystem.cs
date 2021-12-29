using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MessageSystem : MonoBehaviour
{
    private Text dialgueText;
    private string printDialogue = "";
    private string testText = "엄청나게 긴 텍스트 ㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁㅁ";
    private Coroutine typeSentenceCoroutine;
    private static MessageSystem instance;

    public bool IsTypeSetenceRun { get; private set; }

    public static MessageSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MessageSystem>();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        dialgueText = GetComponent<Text>();
        dialgueText.text = printDialogue;

        // StopAllCoroutines();
        // StartCoroutine(TypeSentence(testText));
    }


    /// <summary>
    /// 원하는 문장을 대입하면 한 글자씩 출력됩니다.
    /// </summary>
    /// <param name="sentence"></param> : 출력하고 싶은 문장을 매개변수로 넘겨주세요.
    public void UseTypeSentnece(string sentence)
    {
        if (IsTypeSetenceRun)
        {
            StopTypeSentence(typeSentenceCoroutine);
            IsTypeSetenceRun = false;

            typeSentenceCoroutine = StartCoroutine(TypeSentence(sentence));
        }
        else
        {
            typeSentenceCoroutine = StartCoroutine(TypeSentence(sentence));
        }
    }


    private void StopTypeSentence(Coroutine targetCoroutine)
    {
        StopCoroutine(targetCoroutine);
    }


    private IEnumerator TypeSentence(string sentence)
    {
        printDialogue = "";

        WaitForSeconds waitForSecond = new WaitForSeconds(0.01f);
        IsTypeSetenceRun = true;

        foreach (char letter in sentence.ToCharArray())
        {
            printDialogue += letter;
            dialgueText.text = printDialogue;
            yield return waitForSecond;
        }

        IsTypeSetenceRun = false;
    }
}
