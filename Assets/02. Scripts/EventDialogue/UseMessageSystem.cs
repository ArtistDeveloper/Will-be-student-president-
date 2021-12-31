using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseMessageSystem : MonoBehaviour
{
    private MessageSystem instance;

    private void Start()
    {
        instance = MessageSystem.Instance;
    }

    private void Update()
    {
        // UITextOuputScene에서 Tab을 누를때마다 문자열 자동생성 및 UseTypeSentnece()함수 호출.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var charsArr = new char[30];
            var random = new System.Random();

            for (int i = 0; i < charsArr.Length; i++)
            {
                charsArr[i] = characters[random.Next(characters.Length)];
            }

            var resultString = new String(charsArr);
            UseTypeSentenceExample(resultString);
        }
    }

    private void UseTypeSentenceExample(string sentence)
    {
        instance.UseTypeSentnece(sentence);
    }
}
