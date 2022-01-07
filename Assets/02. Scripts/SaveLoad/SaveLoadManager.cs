using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance = null;
    TestScript testScript;      // -> 나중에 HappeningUtils가 되어야함

    List<Tuple<int,int>> happeningStream;
    int presentHappeningIdx;

    [Serializable]
    public class GameData
    {
        public List<Tuple<int, int>> happeningStream;       // 해프닝 발생 순서
        public int presentHappeningIdx;                     // 현재 진행중인 해프닝 번호
        
        // 추가해야할 사항
        // 사용자의 스탯정보
        // 현재 지지자수(아직 애매)
        // 사용자가 본 대본번호(아직애매)

        // 저장되어있던 순서 반환하는 함수
        // 저장되어있던 이벤트 번호 반환하는 함수
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

        // 이거 나중에 지워야함
        testScript = this.GetComponent<TestScript>();
    }



    // 게임데이터 저장하는 함수
    public void SaveGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        // To DO: 세이브버튼 여러번 누르면 파일이 여러 개 생성되는 지 확인
        // To Do: 파일 여러개에 사용자가 선택해서 넣을 수 있는 지 확인
        FileStream file = File.Create(Application.persistentDataPath + "/gameData.dat");


        GameData gameData = new GameData();

        gameData.happeningStream = testScript.GetHappeningStream();
        gameData.presentHappeningIdx = testScript.GetPresentHappeningIdx();

        Debug.Log("이거 저장한다");
        Debug.Log(testScript.GetHappeningStream().Count);
        Debug.Log(testScript.GetPresentHappeningIdx());
       
        bf.Serialize(file, gameData);
        file.Close();
    }


    // 게임데이터 불러오는 함수
     public void LoadGameData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);
        
        if(file != null && file.Length > 0) 
        {
            // 파일 역질렬화해서 GameData에 담기
            GameData  gameData = (GameData)bf.Deserialize(file);

            happeningStream = gameData.happeningStream;
            presentHappeningIdx = gameData.presentHappeningIdx;

            Debug.Log("이거 불러왔다");
            Debug.Log(happeningStream.Count);
            Debug.Log(presentHappeningIdx);

            // To Do: 여기에 불러왔을 때 게임 진행하는 함수 넣어서 그 함수에서 이벤트 발생순서 등 저장했던 내용 받아가면 될듯
        }

        file.Close();
    }

    public List<Tuple<int, int>> LoadHapeeningStream()
    {
        return happeningStream;
    }

    public int LoadPresentHapeeningIdx()
    {
        return presentHappeningIdx;
    }

}
