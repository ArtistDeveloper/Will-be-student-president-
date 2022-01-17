using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public GameObject settingCanvas;
    
    //SaveLoadManager saveLoadManager;

    // 저장하기 버튼 눌렀을 때 실행되는 함수
    public void OnClickedSaveBtn()
    {
        SaveLoadManager.instance.SaveGameData();
    }

    // 불러오기 버튼 눌렀을 때 실행되는 함수
    public void OnClickedLoadBtn()
    {
        SaveLoadManager.instance.OnClickLoadButton();
    }


    // 뒤로가기 버튼 눌렀을 때 실행되는 함수
    public void OnClickedBackBtn()
    {
        settingCanvas.SetActive(false);
    }
}
