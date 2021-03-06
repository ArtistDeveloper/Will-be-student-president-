using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

public class MakeTextFiles : MonoBehaviour
{
    string path = "Assets/TextData";
    string file;
    string IDs = "1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 50 51 52 70 71 72 100 101 102 103 104 105 106 107 108 109 110 111 112 113 114 120 121 122 123 124 125 126 140 141";
    List<string> eventID = new List<string>();
    public List<string> links = new List<string>();
    StreamWriter sw;
    List<string> contentDatas = new List<string>();
    List<string> result1Datas = new List<string>();
    List<string> result2Datas = new List<string>();
    List<string> choice1Result = new List<string>();
    List<string> choice2Result = new List<string>();
    
    private void Awake() 
    {
        ParcingIDs();
       
        //StartCoroutine(GetDBDatas(links[0], "A2:A", 0, contentDatas, ".txt"));
        //StartCoroutine(GetDBDatas(links[0], "B2:B", 0, result1Datas, "_1.txt"));
        //StartCoroutine(GetDBDatas(links[0], "C2:C", 0, result2Datas, "_2.txt"));
        
        // 지지자 수 관련 파일 생성
        StartCoroutine(GetDBDatas(links[0], "D2:D", 0, choice1Result, "_1"));
        StartCoroutine(GetDBDatas(links[0], "E2:E", 0, choice2Result, "_2"));

    }

    public void ParcingIDs()
    {
        //int IDCnt = IDs.Split(' ').Length;
        for(int i = 0; i<links.Count; i++){
            eventID.Add(IDs.Split(' ')[i]);
        }
    }

    public void MakeFolder(List<string> contentList, string resultType)
    {
        Debug.Log(contentDatas.Count);
        Debug.Log(result1Datas.Count);
        Debug.Log(result2Datas.Count);
        for(int i = 0; i<links.Count; i++){
            //폴더 생성
            Directory.CreateDirectory(path + "/" + eventID[i]);
            file = path + "/" + eventID[i] + "/" + eventID[i];
            if(!File.Exists(file)) {
                string content = contentList[i];

                sw = new StreamWriter(file + resultType);
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
                              
            } else if (File.Exists(file)){
                Directory.Delete(file);
            }
        }
    }

    public void MakeStatResultFolder(List<string> contentList, int choiceNum)
    {
        Directory.CreateDirectory(path + "/statusResult" + choiceNum);
        file = path + "/statusResult" + choiceNum + "/statusResult" + choiceNum +".txt";
        string content = "";
        if(!File.Exists(file)) {
            for(int i = 0; i < contentList.Count; i++) {
                content += "@ " + (i+1).ToString() + "\n" + contentList[i] + "\n"+"\n";
            }
            sw = new StreamWriter(file);
            sw.WriteLine(content); 
            sw.Flush();
            sw.Close();
                            
        } else if (File.Exists(file)){
            Directory.Delete(file);
        }
    }
    // DB에 접속해서 데이터 들고오는 함수
    IEnumerator GetDBDatas(string link, string range, int eventNum, List<string> datas, string resultType)
    {
        Debug.Log("코루틴 호출 수");
        Debug.Log(eventNum);
        
        string URL = link + "/export?format=tsv" + "&gid=0&range=" + range;

        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        
        string data = www.downloadHandler.text; 
        Debug.Log(data);

        if(range != "D2:D" && range !="E2:E")
        {
            datas.Add(ParcingDBData(data));
            eventNum++;
            
            if(eventNum < links.Count)
                StartCoroutine(GetDBDatas(links[eventNum], range, eventNum, datas, resultType));
            else if(eventNum >= links.Count){
                MakeFolder(datas, resultType);
                yield return null; 
            }
        } 
        else 
        {
            datas.Add(ParcingStatData(data));
            eventNum++;
            
            if(eventNum < links.Count) {
                StartCoroutine(GetDBDatas(links[eventNum], range, eventNum, datas, resultType));
            }
            else {
                if(resultType == "_1"){
                    MakeStatResultFolder(datas, 1);
                }
                else {
                    MakeStatResultFolder(datas, 2);
                }
                yield return null;
            }

        }
    }

    private string ParcingDBData(string data)
    {
        string result = "";
        for(int i = 0; i < data.Split('\n').Length; i++){
            result += data.Split('\n')[i] + "\n";
        }
        return result;
    }

    
    private string ParcingStatData(string data)
    {
        if(data!=null){
            string choiceResult = "";
            for(int i = 0; i < data.Split('\n').Length; i++)
            {
            choiceResult += data.Split('\n')[i] + "\n";
            }
            return choiceResult;
        } else return "";
    }
}
