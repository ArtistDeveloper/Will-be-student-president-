//#define STATUS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GamePlaySceneSettings : MonoBehaviour
{
    public Button scriptNextButton, scriptBackButton, saveButton;
    public GameObject nextDayWarningPanel;
    public GameObject message;
    // GamePlayScene(Status)의 MainScreenCanvas->StatusPanel의 텍스트들을 차례로 넣어주시면 됩니다.
    public Text networkingText;
    public Text eloquenceText;
    public Text reputationText;
    public Text moneyText;
    public Text approvalRatingText;
    // Start is called before the first frame update
    void Start()
    {
        // 대화 넘기기 버튼과 GetNextHappening의 PrintNextScripts 연결
        scriptNextButton.onClick.AddListener(GetNextHappening.instance.PrintNextScripts);
        // 대화 뒤로가기 버튼과 GetNextHappening의 PrintBackScripts 연결
        scriptBackButton.onClick.AddListener(GetNextHappening.instance.PrintBackScripts);

        // Save 버튼과 SaveLoadManager의 SaveGameData와 연결
        saveButton.onClick.AddListener(SaveLoadManager.instance.SaveGameData);

        // 한번더누르면 다음날 안내 panel과 GetNextHappening의 nextDayWarning 연결
        GetNextHappening.instance.nextDayWarning = nextDayWarningPanel;
        // 아래 대화 텍스트와 GetNextHappening의 dialogText 연결
        GetNextHappening.instance.dialogText = message.GetComponent<Text>();
#if STATUS
        StatusManager.instance.SetTextComponent(networkingText, eloquenceText, reputationText, moneyText, approvalRatingText);
        StatusManager.instance.ApplyStatusToText();
#endif
    }
}
