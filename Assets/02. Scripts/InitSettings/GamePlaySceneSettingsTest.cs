using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GamePlaySceneSettingsTest : MonoBehaviour
{
    // GamePlaySceneSettings.cs의 내용을 이 코드로 바꿔야 합니다.
    public Button scriptNextButton, scriptBackButton, saveButton;
    public GameObject nextDayWarningPanel;
    public GameObject message;
    // GamePlayScene(Status)의 MainScreenCanvas->StatusPanel의 텍스트들을 차례로 넣어주시면 됩니다.
    public Text networkingText;
    public Text eloquenceText;
    public Text reputationText;
    public Text moneyText;
    public Text approvalRatingText;
    //public Button yesButton;


    // Start is called before the first frame update
    void Start()
    {
        // 대화 넘기기 버튼과 GetNextHappening의 PrintNextScripts 연결
        scriptNextButton.onClick.AddListener(ScenarioMaster.instance.OnClickNextButton);
        // 대화 뒤로가기 버튼과 GetNextHappening의 PrintBackScripts 연결
        scriptBackButton.onClick.AddListener(ScenarioMaster.instance.OnClickBackButton);

        // Save 버튼과 SaveLoadManager의 SaveGameData와 연결
        saveButton.onClick.AddListener(SaveLoadManager.instance.SaveGameData);

        // 한번더누르면 다음날 안내 panel과 GetNextHappening의 nextDayWarning 연결
        ScenarioMaster.instance.nextDayWarning = nextDayWarningPanel;
        // 아래 대화 텍스트와 GetNextHappening의 dialogText 연결
        ScenarioMaster.instance.dialogText = message.GetComponent<Text>();
        StatusManager.instance.SetTextComponent(networkingText, eloquenceText, reputationText, moneyText, approvalRatingText);

        //yesButton.onClick.AddListener(ChoiceManager.instance.ExitChoice);

        ScenarioMaster.instance.settingFlag = true; // GameManager로 옮겨야됨
        StatusManager.instance.settingFlag = true; // GameManager로 옮겨야됨. 위에 bool변수랑 합치기
    }
}
