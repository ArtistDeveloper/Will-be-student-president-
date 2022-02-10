//#define SCENARIO_MASTER_TEST // 지워야 할 내용
//#define STATUS // 지워야 할 내용
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance = null;

    List<Tuple<int,int>> savedHappeningStream;
    int savedPresentHappeningIdx;

    [Serializable]
    public class GameData
    {
        public List<Tuple<int, int>> happeningStream;       // 해프닝 발생 순서
        public int presentHappeningIdx;                     // 현재 진행중인 해프닝 번호
        public List<string> txtScript;
        #if SCENARIO_MASTER_TEST// 지워야 할 내용
        public Queue<string> commandLines;
        #else// 지워야 할 내용
        public bool branchFlag;// 지워야 할 내용
        public string question;// 지워야 할 내용
        public List<string> answerList;// 지워야 할 내용
        public List<string> branchFilePath;// 지워야 할 내용
        #endif// 지워야 할 내용

        public Tuple<int, int, int, int> playerStatus;       // 사용자의 스탯(인맥, 언변, 평판, 자금)

        #if SCENARIO_MASTER_TEST// 지워야 할 내용
        #else// 지워야 할 내용
        public List<Tuple<int, int, int, int>> choiceStatus;   // 선택지의 스탯(인맥, 언변, 평판, 자금) // 지워야 할 내용
        #endif// 지워야 할 내용
        // 추가해야할 사항
        // 사용자가 본 대본번호(아직애매)
    }

    private void Awake() 
    {
        // 싱글톤
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

    }



    // 게임데이터 저장하는 함수
    public void SaveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        // To DO: 세이브버튼 여러번 누르면 파일이 여러 개 생성되는 지 확인
        // To Do: 파일 여러개에 사용자가 선택해서 넣을 수 있는 지 확인
        string path = Path.Combine(Application.persistentDataPath, "gameDataSave.dat");
        FileStream file = File.Create(path);

        GameData gameData = new GameData();

        gameData.happeningStream = HappeningUtils.instance.GetHappeningStream();
        gameData.presentHappeningIdx = HappeningUtils.instance.GetPresentHappeningIdx();

        #if SCENARIO_MASTER_TEST// 지워야 할 내용
        gameData.txtScript = ScenarioMaster.instance.Get_txtScripts();
        gameData.commandLines = ScenarioMaster.instance.Get_commandLines();
        #else// 지워야 할 내용
        gameData.txtScript = GetNextHappening.instance.Get_txtScripts();// 지워야 할 내용
        gameData.branchFlag = GetNextHappening.instance.Get_branchFlag();// 지워야 할 내용
        gameData.question = GetNextHappening.instance.Get_question();// 지워야 할 내용
        gameData.answerList = GetNextHappening.instance.Get_answerList();// 지워야 할 내용
        gameData.branchFilePath = GetNextHappening.instance.Get_branchFilePath();// 지워야 할 내용
        #endif// 지워야 할 내용

        #if STATUS// 지워야 할 내용
        gameData.playerStatus = StatusManager.instance.SaveStatus();
        #if SCENARIO_MASTER_TEST// 지워야 할 내용
        #else// 지워야 할 내용
        gameData.choiceStatus = GetNextHappening.instance.Get_statusValue();// 지워야 할 내용
        #endif// 지워야 할 내용
        #endif// 지워야 할 내용

        Debug.Log("이거 저장한다");
        Debug.Log(HappeningUtils.instance.GetHappeningStream().Count);
        Debug.Log(HappeningUtils.instance.GetPresentHappeningIdx());
       
        bf.Serialize(file, gameData);
        file.Close();
    }

    // Start 버튼 눌렀을 때 onClick 함수
    public void OnClickStartButton(){
        HappeningUtils.instance.MakeNewProgress();
        #if SCENARIO_MASTER_TEST// 지워야 할 내용
        ScenarioMaster.instance.InitSettings();
        #else// 지워야 할 내용
        GetNextHappening.instance.InitSettings();// 지워야 할 내용
        #endif// 지워야 할 내용
        #if STATUS// 지워야 할 내용
        StatusManager.instance.SetStatusValue(5,5,5,5); // 스텟 초기값 설정
        #endif// 지워야 할 내용
    }


    // Load 버튼 눌렀을 때 onClick 함수
    // 게임데이터 불러오는 함수
    public void OnClickLoadButton()
    {
        try{
            BinaryFormatter bf = new BinaryFormatter();
            string path = Path.Combine(Application.persistentDataPath, "gameDataSave.dat");
            FileStream file = File.OpenRead(path);
            
            if(file != null && file.Length > 0) 
            {
                // 파일 역질렬화해서 GameData에 담기
                GameData  gameData = (GameData)bf.Deserialize(file);

                // To Do: HappeningUtils로 넘기기
                HappeningUtils.instance.SetHappeningStream(gameData.happeningStream);
                HappeningUtils.instance.SetPresentHappeningIdx(gameData.presentHappeningIdx);

                
                #if SCENARIO_MASTER_TEST// 지워야 할 내용
                ScenarioMaster.instance.Set_txtScripts(gameData.txtScript);
                ScenarioMaster.instance.Set_commandLines(gameData.commandLines);
                #else// 지워야 할 내용
                GetNextHappening.instance.Set_txtScripts(gameData.txtScript);// 지워야 할 내용
                GetNextHappening.instance.Set_branchFlag(gameData.branchFlag);// 지워야 할 내용
                GetNextHappening.instance.Set_question(gameData.question);// 지워야 할 내용
                GetNextHappening.instance.Set_answerList(gameData.answerList);// 지워야 할 내용
                GetNextHappening.instance.Set_branchFilePath(gameData.branchFilePath);// 지워야 할 내용
                #endif// 지워야 할 내용
                
                #if STATUS// 지워야 할 내용
                StatusManager.instance.LoadStatus(gameData.playerStatus);
                #if SCENARIO_MASTER_TEST// 지워야 할 내용
                #else// 지워야 할 내용
                GetNextHappening.instance.Set_statusValue(gameData.choiceStatus);// 지워야 할 내용
                #endif// 지워야 할 내용
                #endif// 지워야 할 내용


                Debug.Log("이거 불러왔다");
                Debug.Log("HappeningUtils에 저장된거");
                Debug.Log(HappeningUtils.instance.GetHappeningStream().Count);
                Debug.Log(HappeningUtils.instance.GetPresentHappeningIdx());


                // To Do: 여기에 불러왔을 때 게임 진행하는 함수 넣어서 그 함수에서 이벤트 발생순서 등 저장했던 내용 받아가면 될듯
            }

            file.Close();
        }
        catch(Exception e) {
            Debug.Log("로드에러메시지");
            Debug.Log(e.Message);
        }
    }
}
