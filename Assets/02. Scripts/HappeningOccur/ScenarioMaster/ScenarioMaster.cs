using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ScenarioMaster : MonoBehaviour
{
    public static ScenarioMaster instance = null;
    private void Awake()
    {
        if (instance == null)
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

    // ANCHOR top
    public string defaultFolderPath; // 이벤트 대화들을 저장하는 파일들의 기본 폴더 경로
    public enum commandType { text, endtext, get_status, branch, bg, error }; // 명령어 type


    // 저장해야 할 내용들
    private List<string> txtScripts; // 이벤트 대화들을 저장하는 리스트
    private int txtScriptsIndex; // 현재 몇번째 대화를 보고 있는지
    private List<string> branchFilePath; // 다음 대화로 연결되는 대화 스크립트 경로
    bool choiceIng; // 선택지 창 켜져있는지
    private Queue<string> commandLines; // txt 명령어 라인들
    private Queue<string> branchCache, commandCaches;

    private bool endText;


    public Text dialogText; // 게임 화면 맨 아래 텍스트 창
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    public GameObject nextDayWarning; // 다음날로 넘어가려면 버튼 한번 더 안내 Panel
    bool nextDayFlag = false; // true인 상태에서 다음대화 한번 더 누르면 다음날로 넘어가는 flag

    public bool settingFlag;


    private void Start()
    {
        txtScripts = new List<string>();
        branchFilePath = new List<string>() { "", "", "", "", "" };
        commandLines = new Queue<string>();
        choiceIng = false;
        txtScriptsIndex = 0;
        branchCache = new Queue<string>();
        commandCaches = new Queue<string>();

        settingFlag = false;
    }

    public void InitSettings()
    {
        txtScripts.Clear();
        txtScriptsIndex = 0;
        choiceIng = false;
        commandLines.Clear();
        endText = true;
        branchCache.Clear();
        commandCaches.Clear();

        ReadPresentHappening();
    }




    // ANCHOR 현재 이벤트 로딩 - ReadPresentHappening
    /// <summary>
    /// 현재 이벤트 텍스트 파일 id.txt를 읽어옴
    /// </summary>
    public void ReadPresentHappening()
    {
        if (choiceIng) return;
        txtScripts.Clear(); // 대화 로그 초기화
        txtScriptsIndex = 0; // 대화 로그 번호 초기화

        HappeningUtils.instance.DebugPrintHappening__(
            HappeningUtils.instance.GetPresentHappening__());

        endText = false;
        ParseTextScript(
            MakeFilePath(HappeningUtils.instance.GetPresentHappening__().Item2.ToString())
        );
    }


    // ANCHOR 텍스트파일 줄단위 파싱 - ParseTextScript
    /// <summary>
    /// 주석, 빈 줄 등을 제거하여
    /// commandLines에 명령어들을 저장하는 함수
    /// </summary>
    /// <param name="filePath"></param>
    private void ParseTextScript(string filePath)
    {
        try
        {
            StreamReader happeningsTxtScripts = new StreamReader(
                new FileStream(filePath, FileMode.Open));
            string line = "";
            while (happeningsTxtScripts.EndOfStream != true)
            {
                line = happeningsTxtScripts.ReadLine();
                if (line.Length <= 0) continue;
                if (line.Length >= 2 && line[0] == '/' && line[1] == '/') continue;
                commandLines.Enqueue(line.Trim());
            }
            happeningsTxtScripts.Close();
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e);
            txtScripts.Add("해당 대화 스크립트가 존재하지 않습니다.");
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log(e);
            txtScripts.Add("해당 대화 스크립트 폴더가 존재하지 않습니다.");
        }
        catch (IOException e)
        {
            Debug.Log(e);
            txtScripts.Add("대화 스크립트 형식이 잘못되었습니다.");
        }
        choiceIng = false;
    }


    // ANCHOR 명령어 실행 함수 - ExecuteCommand
    /// <summary>
    /// 명령어 실행 함수
    /// </summary>
    /// <param name="command">Queue<string> commandLines의 front</param>
    /// <returns>commandType(명령어 타입 enum)</returns>
    private commandType ExecuteCommand(string command)
    {
        string[] cmdtmp = command.Split(' ');
        string type = cmdtmp[0];
        string contents = "";
        if (cmdtmp.Count() > 1)
        {
            contents = command.Substring(command.IndexOf(" ") + 1);
        }
        switch (type)
        {
            case "text":
                txtScripts.Add(contents);
                commandCaches.Enqueue(command);
                return commandType.text;
            case "endtext":
                endText = true;
                commandCaches.Enqueue(command);
                return commandType.endtext;
            case "getstatus":
                cmdtmp = contents.Split(' ');
                StatusManager.instance.ChangeStatus(
                    Int32.Parse(cmdtmp[0]),
                    Int32.Parse(cmdtmp[1]),
                    Int32.Parse(cmdtmp[2]),
                    Int32.Parse(cmdtmp[3])
                );
                return commandType.get_status;
            case "branch":
                branchCache.Enqueue(command);
                int count = int.Parse(cmdtmp[1]); // 선택지 개수
                ChoiceManager.instance.Set_question(command.Substring(command.IndexOf(cmdtmp[1]) + 2));
                // 선택지 텍스트 채우기
                ChoiceManager.instance.Clear_answerList(); // IndexOutOfRange 에러 해결 코드
                for (int i = 0; i < count; i++)
                {
                    branchCache.Enqueue(commandLines.Peek());
                    ChoiceManager.instance.Add_answerList(commandLines.Dequeue());
                }
                // 분기 파일 이름 채우기
                for (int i = 0; i < count; i++)
                {
                    branchCache.Enqueue(commandLines.Peek());
                    branchFilePath[i] = commandLines.Dequeue();
                }
                StartCoroutine(BranchWaitCoroutine());
                return commandType.branch;
            case "bg":
                // 백그라운드클래스.instance.함수(cmdtmp[1]); // 주석해제, 클래스명, 함수명 바꾸기
                commandCaches.Enqueue(command);
                return commandType.bg;
            default:
                return commandType.error;
        }

    }

    // ANCHOR 다음 대화 버튼 - OnClickNextButton
    /// <summary>
    /// 다음 대화 버튼을 눌렀을 때
    /// 다음 대화를 출력
    /// </summary>
    public void OnClickNextButton()
    {
        if (txtScripts == null) return;
        if (choiceIng) return; // 선택하고 있을 때는 대화를 못움직이게

        if (endText == false && txtScriptsIndex == txtScripts.Count)
        {
            while (commandLines.Count != 0)
            {
                string command = commandLines.Dequeue();
                commandType cmdtype = ExecuteCommand(command);
                Debug.Log(cmdtype);
                if (cmdtype == commandType.text || cmdtype == commandType.endtext)
                {
                    PrintScript();
                    break;
                }
                else if (cmdtype == commandType.branch)
                {
                    break;
                }
                else if(cmdtype == commandType.error)
                {
                    Debug.Log(cmdtype);
                    break;
                }
                else{
                    continue;
                }
            }
        }
        else
        {
            PrintScript();
        }
    }

    // ANCHOR 이전 대화 버튼 - OnClickBackButton
    /// <summary>
    /// 이전 대화를 출력 
    /// </summary>
    public void OnClickBackButton()
    {
        if (txtScripts == null) return;
        if (choiceIng) return; // 선택하고 있을 때는 대화를 못움직이게
        PrintScript(true);
    }


    // ANCHOR 대화 출력 관리 함수 - PrintScript
    /// <summary>
    /// 
    /// </summary>
    /// <param name="backLog">이전대화를 출력</param>
    /// <param name="present">현재 대화를 출력</param>
    public void PrintScript(bool backLog = false, bool present = false)
    {
        if(present){
            if (txtScriptsIndex > 0)
            {
                Debug.Log("" + txtScriptsIndex + "번째 대화 | " + txtScripts[txtScriptsIndex - 1]);
                StartCoroutine(TypingDialogCoroutine(txtScripts[txtScriptsIndex - 1]));
            }
        }
        else if (backLog == false)
        {
            if (txtScriptsIndex < txtScripts.Count)
            {
                Debug.Log("" + txtScriptsIndex + "번째 대화 | " + txtScripts[txtScriptsIndex]);
                StartCoroutine(TypingDialogCoroutine(txtScripts[txtScriptsIndex]));
                txtScriptsIndex++;
                nextDayFlag = false;
            }
            else
            {
                // 대화를 끝까지 읽은 후 선택지가 없을 때,
                // 다음 대화를 2번 누르면 다음날로 넘어갈 수 있는 기능
                if (nextDayFlag)
                {
                    HappeningUtils.instance.IncreaseHappeningIdx();
                 ReadPresentHappening();
                    nextDayFlag = false;
                }
                else
                {
                    Debug.Log("다음날로 넘어가려면 한번 더 누르세요.");
                    nextDayFlag = true;
                }
                nextDayWarning.SetActive(nextDayFlag);
            }
        }
        else
        {
            if (0 <= txtScriptsIndex - 2)
            {
                txtScriptsIndex -= 2;
                Debug.Log("" + txtScriptsIndex + "번째 대화 | " + txtScripts[txtScriptsIndex]);
                StartCoroutine(TypingDialogCoroutine(txtScripts[txtScriptsIndex]));
                txtScriptsIndex++;
            }
            nextDayFlag = false;
            nextDayWarning.SetActive(nextDayFlag);
        }
    }



    private IEnumerator TypingDialogCoroutine(string line)
    {
        dialogText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += line[i];
            yield return waitTime;
        }
    }

    // ANCHOR BranchWaitCoroutine
    /// <summary>
    /// 대화 파일을 읽었을 때 BRANCH라는 선택지 플래그가 있을 때,
    /// branchFlag가 true가 되고, 대화를 끝까지 읽은 후 선택지 창이 나올 때
    /// 실행되어 ChoiceManager에서 선택지 선택을 완료할 때 까지 기다리고
    /// 각 선택지에 맞는 선택 후 대화 파일을 읽어 출력할 수 있도록
    /// MakeFilePath에 파일 이름을 전달한다.
    /// </summary>
    /// <returns>ChoiceManager에서 선택을 완료할 때 까지 기다리기</returns>
    private IEnumerator BranchWaitCoroutine()
    {
        choiceIng = true;
        ChoiceManager.instance.ShowChoice();
        yield return new WaitUntil(() => !ChoiceManager.instance.choiceIng);
        branchCache.Clear();
        ParseTextScript(
            MakeFilePath(branchFilePath[ChoiceManager.instance.GetResult()])
        );
        yield return new WaitUntil(() => !choiceIng);
    }



    private string MakeFilePath(string key)
    {
        string[] tmp = key.Split('_');
        return (defaultFolderPath + '/' + tmp[0] + '/' + key + ".txt");
    }


    // ANCHOR 로딩 관련 함수들
    public void Set_commandLines(Queue<string> data){
        txtScriptsIndex = 0;
        txtScripts.Clear();
        branchCache.Clear();
        commandCaches.Clear();
        commandLines = data;
    }
    public void Set_commandCachesCount(int data)
    {
        for (int i = 0; i < data; i++)
        {
            ExecuteCommand(commandLines.Dequeue());
        }
        txtScriptsIndex = txtScripts.Count;
        nextDayFlag = false;
        PrintScript(present: true);
    }

    // ANCHOR 저장 관련 함수들
    public Queue<string> Get_commandLines(){
        // 오류 수정
        Queue<string> ret = new Queue<string>(), tmp;

        tmp = new Queue<string>(commandCaches);
        foreach(var cmd in tmp){
            ret.Enqueue(cmd);
        }

        tmp = new Queue<string>(branchCache);
        foreach(var cmd in tmp){
            ret.Enqueue(cmd);
        }

        tmp = new Queue<string>(commandLines);
        foreach(var cmd in tmp){
            ret.Enqueue(cmd);
        }
        return ret;
    }
    public int Get_commandCachesCount(){
        return commandCaches.Count;
    }
    
}
