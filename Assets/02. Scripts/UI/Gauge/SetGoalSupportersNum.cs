using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class SetGoalSupportersNum : MonoBehaviour
{
  public static SetGoalSupportersNum instance = null;
  const string EventNumSign = "@";
  const string GetStatus = "getstatus";
  string[] filePath = new string[2]{
    "Assets/TextData/statusResult1/statusResult1.txt",
    "Assets/TextData/statusResult2/statusResult2.txt"
  };
  int[,] choice1StatusResult = new int[64,4];    // 선택지 1번을 선택했을 때 변동되는 스테이터스 값 (이벤트번호(스테이터스(인맥,언변,평판,돈)))
  int[,] choice2StatusResult = new int[64,4];    // 선택지 2번을 선택했을 때 변동되는 스테이터스 값 (이벤트번호(스테이터스(인맥,언변,평판,돈)))  

  string IDs = "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 50 51 52 70 71 72 100 101 102 103 104 105 106 107 108 109 110 111 112 113 114 120 121 122 123 124 125 126 140 141";
  
  Dictionary<int, int> eventID = new Dictionary<int, int>();

  List<Tuple<int,int>> updatedHappeningStream = new List<Tuple<int, int>>();  // 정렬된 이벤트 순서
  List<float> sortedSupporters = new List<float>();        // 정렬된 스테이터스 결과 값
  List<float> stageSupporters = new List<float>();
  int eventNum = 64;  // 이벤트 개수
  public int maxStageNum = 4;  // 스테이지 개수
  int stageIdx = 0;         // 스테이지 번호
  int stageEventNum;    // 스테이지당 이벤트 개수
  public float networkingContribution;
  public float eloquenceContribution;
  public float reputationContribution;
  public float moneyContribution;
  
  void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Util Class created successfully");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Util Class alread exists!");
            Destroy(this.gameObject);
        }
    }

  void Start()
  {
     stageEventNum = eventNum/maxStageNum;
    ParcingEventIDs();

    // 우선 파일 내용 읽고 배열에 저장해봅시다
    ReadLineStatutsResult(filePath[0], choice1StatusResult);
    ReadLineStatutsResult(filePath[1], choice2StatusResult);
    InitStageSupporters();
  }

  public void InitStageSupporters()
  {
    SetSortedSupporters();
    
    SetStageSupporterse();
    
    for(int i = 0; i < sortedSupporters.Count; i++)
    {
      Debug.Log("정렬된 지지자수");
      Debug.Log(i);
      Debug.Log(sortedSupporters[i]);
    }

     for(int i = 0; i < stageSupporters.Count; i++)
    {
      Debug.Log("스테이지별 지지자수");
      Debug.Log(i);
      Debug.Log(stageSupporters[i]);
    }
    
  }

   public void ParcingEventIDs()
    {
        //int IDCnt = IDs.Split(' ').Length;
        for(int i = 0; i < 64; i++){
            eventID.Add(int.Parse(IDs.Split(' ')[i]), i);
        }
    }

  
  // 파일에 있는 내용 한줄 씩 읽는 함수
  void ReadLineStatutsResult(string filePath, int[,] statusResult)
  {
    StreamReader StautsResultStream = new StreamReader(new FileStream(filePath, FileMode.Open));
    string line = "";
    int idx = -1;
    while (StautsResultStream.EndOfStream != true)
    {
      line = StautsResultStream.ReadLine();
      if(line.Split(' ')[0] == EventNumSign) 
      {
        idx++;
        line = StautsResultStream.ReadLine();
        while(line.Split(' ')[0] == GetStatus)
        {
          // 스테이터스 결과 배열에 추가
          for(int i = 0; i< 4; i++) 
          {
            statusResult[idx,i] += int.Parse(line.Split(' ')[i+1]);
          }
          line = StautsResultStream.ReadLine();
        } 
      } else continue; 
    } 
  }
  
  // 랜덤생성된 이벤테 발생 순서 받아와서 스테이지별로 이벤트 나눠줘야하는뎅..
  void SetSortedSupporters()
  {
    while(updatedHappeningStream.Count == 0) 
    {
      updatedHappeningStream = HappeningUtils.instance.GetHappeningStream();
    } 
    Debug.Log("이벤트 발생순서 불러옴");
    Debug.Log(updatedHappeningStream.Count);
    eventNum = updatedHappeningStream.Count;

    for(int i = 0; i < updatedHappeningStream.Count; i++) 
    {
      int key = updatedHappeningStream[i].Item2;
      Debug.Log("발생하는 이벤트 ID");
      Debug.Log(key);
      int value;
      if(eventID.TryGetValue(key, out value))
      {
        Debug.Log(value);
        float choice1Supporters = GetMaxSupporters(value, choice1StatusResult);
        float choice2Supporters = GetMaxSupporters(value, choice2StatusResult);
        // 지지자수가 더 높은 선택지의 지지자수를 이벤트 발생순서대로 저장
        if(choice1Supporters >= choice2Supporters)
        {
          sortedSupporters.Add(choice1Supporters);
        } else {
          sortedSupporters.Add(choice2Supporters);
        }
      }
    }
    Debug.Log("sortedSupporters개수");
    Debug.Log(sortedSupporters.Count);

  }

   // 선택지에 따른 지지자수 크기 비교해서 더 큰 지지자수 반환
  float GetMaxSupporters(int eventIdx, int[,] choiceStatusResult)
  {
    Debug.Log("최대 지지자수 계산");
    float supporters = 0;
    float networking, eloquence, reputation, money = 0;
        
    networking = choiceStatusResult[eventIdx, 0];
    eloquence = choiceStatusResult[eventIdx, 1];
    reputation = choiceStatusResult[eventIdx, 2];
    money = choiceStatusResult[eventIdx, 3];
    
    supporters = networking * networkingContribution + eloquence * eloquenceContribution + reputation * reputationContribution + money * moneyContribution;

    return supporters;
  }
  
  // 스테이지별 지지자수 초기화
  void SetStageSupporterse()
  {
    float goalSupporters;
    stageEventNum = eventNum/maxStageNum;

    for(int i = 0; i< maxStageNum; i++)
    {
      goalSupporters = 0;
      for(int j = 0; j <stageEventNum; j++)
      {
        goalSupporters += sortedSupporters[j + maxStageNum*i];
      }
      goalSupporters *= 0.8f;
      stageSupporters.Add(goalSupporters);
    }
  }
  
  public List<float> GetStageSupporters()
  {
    return stageSupporters;
  }
}
