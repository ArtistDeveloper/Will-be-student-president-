using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlotManager : MonoBehaviour
{
    const string targetScene = "LoadingScene";
    public static SlotManager instance = null;
    public int slotCount;           // 슬롯 개수
    public GameObject SaveSlotContent, SaveSlotView;      // 저장 슬롯 버튼이 생성될 공간
    public GameObject LoadSlotContent, LoadSlotView;      // 불러오기 슬롯 버튼이 생성될 공간
    public GameObject slotPrefab;   // 만들어진 슬롯 프리팹

    private void Awake() 
    {
        #region 싱글톤
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        #endregion

        this.gameObject.SetActive(false);
    }

    
    // 저장용 버튼 슬롯 보이게하는 함수
    public void OpenSaveSlotWindow()
    {
        Debug.Log("저장슬롯창열기");
        LoadSlotView.SetActive(false);
        SaveSlotView.SetActive(true);
        this.gameObject.SetActive(true);
    }
    // 저장용 버튼 슬롯 안보이게하는 함수
    public void CloseSaveSlotWindow()
    {
        Debug.Log("저장슬롯창닫기");
        SaveSlotView.SetActive(false);
    }
    // 불러오기용 버튼 슬롯 보이게하는 함수
    public void OpenLoadSlotWindow()
    {
        Debug.Log("불로오기슬롯창열기");
        SaveSlotView.SetActive(false);
        LoadSlotView.SetActive(true);
        this.gameObject.SetActive(true);
    }
    // 불러오기용 버튼 슬롯 보이게하는 함수
    public void CloseLoadSlotWindow()
    {
        Debug.Log("불로오기슬롯창닫기");
        LoadSlotView.SetActive(false);
    }

    // 저장용 버튼 슬롯 동적 생성
    public void CreateSaveSlotButtons() 
    {
        Button slotButton;              
        GameObject child;
        if(SaveSlotContent.GetComponentsInChildren<Transform>().Length <= 1) {
            for(int i = 0; i < slotCount; i++) {
                int tmp = i;
                child = Instantiate(slotPrefab);
                child.transform.SetParent(SaveSlotContent.transform);
            
                slotButton = child.GetComponent<Button>();
                slotButton.onClick.AddListener(() => SaveLoadManager.instance.SaveGameData(tmp));
                
            }   
        }
    }

    public void CreateLoadSlotButtons() 
    {   
        Button slotButton;              
        GameObject child;
        if(LoadSlotContent.GetComponentsInChildren<Transform>().Length <= 1) {
            for(int i = 0; i < slotCount; i++) {
                int tmp = i;
                child = Instantiate(slotPrefab);
                child.transform.SetParent(LoadSlotContent.transform);
            
                slotButton = child.GetComponent<Button>();
                slotButton.onClick.AddListener(() => SaveLoadManager.instance.OnClickLoadButton(tmp));
                slotButton.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
                slotButton.onClick.AddListener(() => CloseSlotWindow());
            } 
        }
    }

    public void OpenSlotWindow()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseSlotWindow()
    {
        this.gameObject.SetActive(false);
        CloseSaveSlotWindow();
        CloseLoadSlotWindow();
    }


 
}
