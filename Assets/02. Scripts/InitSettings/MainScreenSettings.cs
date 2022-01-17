using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainScreenSettings : MonoBehaviour
{
    public Button startButton, loadButton;
    // Start is called before the first frame update
    void Start()
    {
        // SaveLoadManager의 onClickStartButton과 MainScreen의 Start 버튼 연결
        startButton.onClick.AddListener(SaveLoadManager.instance.OnClickStartButton);
        // SaveLoadManager의 onClickLoadButton과 MainScreen의 Load 버튼 연결
        loadButton.onClick.AddListener(SaveLoadManager.instance.OnClickLoadButton);
    }
}
