using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetBgTest : MonoBehaviour
{
    public Sprite sprite;

    private void Start()
    {
        sprite = BackgroundManager.Instance.GetBackground("Test");
    }
}
