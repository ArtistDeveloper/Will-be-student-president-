using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance = null;
    public GetNextHappening getNextHappening;

    List<Tuple<int,int>> savedHappeningStream;
    int savedPresentHappeningIdx;

    [Serializable]
    public class GameData
    {
        public List<Tuple<int, int>> happeningStream;       // 해프닝 발생 순서
        public int presentHappeningIdx;                     // 현재 진행중인 해프닝 번호
        public List<string> txtScript;
        public bool branchFlag;
        public string question;
        public List<string> answerList;
        public List<string> branchFilePath;

        // 추가해야할 사항
        // 사용자의 스탯정보
        // 현재 지지자수(아직 애매)
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
            if(instance != this)
                Destroy(this.gameObject);
        }

    }



    // 게임데이터 저장하는 함수
    public void SaveGameData()
    {
        getNextHappening = GameObject.Find("NextDayButton").GetComponent<GetNextHappening>();
        BinaryFormatter bf = new BinaryFormatter();
        // To DO: 세이브버튼 여러번 누르면 파일이 여러 개 생성되는 지 확인
        // To Do: 파일 여러개에 사용자가 선택해서 넣을 수 있는 지 확인
        string path = Path.Combine(Application.persistentDataPath, "gameDataSave.dat");
        FileStream file = File.Create(path);

        GameData gameData = new GameData();

        gameData.happeningStream = HappeningUtils.instance.GetHappeningStream();
        gameData.presentHappeningIdx = HappeningUtils.instance.GetPresentHappeningIdx();
        gameData.txtScript = getNextHappening.Get_txtScripts();
        gameData.branchFlag = getNextHappening.Get_branchFlag();
        gameData.question = getNextHappening.Get_question();
        gameData.answerList = getNextHappening.Get_answerList();
        gameData.branchFilePath = getNextHappening.Get_branchFilePath();

        Debug.Log("이거 저장한다");
        Debug.Log(HappeningUtils.instance.GetHappeningStream().Count);
        Debug.Log(HappeningUtils.instance.GetPresentHappeningIdx());
       
        bf.Serialize(file, gameData);
        file.Close();
    }


    // 게임데이터 불러오는 함수
     public void LoadGameData()
    {
        try{
            getNextHappening = GameObject.Find("NextDayButton").GetComponent<GetNextHappening>();
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
                getNextHappening.Set_txtScripts(gameData.txtScript);
                getNextHappening.Set_branchFlag(gameData.branchFlag);
                getNextHappening.Set_question(gameData.question);
                getNextHappening.Set_answerList(gameData.answerList);
                getNextHappening.Set_branchFilePath(gameData.branchFilePath);

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
