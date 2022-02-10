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
    public enum commandType { text, endtext, get_status, branch, error }; // 명령어 type


    // 저장해야 할 내용들
    private List<string> txtScripts; // 이벤트 대화들을 저장하는 리스트
    private int txtScriptsIndex; // 현재 몇번째 대화를 보고 있는지
    private List<string> branchFilePath; // 다음 대화로 연결되는 대화 스크립트 경로
    bool choiceIng; // 선택지 창 켜져있는지
    private Queue<string> commandLines; // txt 명령어 라인들
    private Queue<string> branchCache;

    private bool endText;


    public Text dialogText; // 게임 화면 맨 아래 텍스트 창
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    public GameObject nextDayWarning; // 다음날로 넘어가려면 버튼 한번 더 안내 Panel
    bool nextDayFlag = false; // true인 상태에서 다음대화 한번 더 누르면 다음날로 넘어가는 flag


    private void Start()
    {
        txtScripts = new List<string>();
        branchFilePath = new List<string>() { "", "", "", "", "" };
        commandLines = new Queue<string>();
        choiceIng = false;
        txtScriptsIndex = 0;
        branchCache = new Queue<string>();
    }

    public void InitSettings()
    {
        txtScripts.Clear();
        txtScriptsIndex = 0;
        choiceIng = false;
        commandLines.Clear();
        endText = true;
        branchCache.Clear();
    }




    // ANCHOR GetNext
    /// <summary>
    /// 다음 이벤트를 읽어옴.
    /// </summary>
    public void ReadNextHappening()
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
        HappeningUtils.instance.IncreaseHappeningIdx(); // 이거를 다음날 넘어가는 직전에 증가시키는거로 변경해야될듯
    }



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
                return commandType.text;
            case "endtext":
                endText = true;
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
            default:
                return commandType.error;
        }

    }

    // ANCHOR PrintNextScripts
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
                if (cmdtype == commandType.text)
                {
                    PrintScript();
                    break;
                }
                else if (cmdtype == commandType.branch)
                {
                    break;
                }
                else if (cmdtype == commandType.endtext)
                {
                    PrintScript();
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

    // ANCHOR PrintBackScripts
    /// <summary>
    /// 이전 대화를 출력 
    /// </summary>
    public void OnClickBackButton()
    {
        if (txtScripts == null) return;
        if (choiceIng) return; // 선택하고 있을 때는 대화를 못움직이게
        PrintScript(true);
    }

    public void PrintScript(bool backLog = false)
    {
        if (backLog == false)
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
                    ReadNextHappening();
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


    // ANCHOR InitFunctions
    public void Set_txtScripts(List<string> data)
    {
        txtScriptsIndex = 0;
        txtScripts.Clear();
        foreach (var line in data)
        {
            txtScripts.Add(line);
        }
    }
    public void Set_commandLines(Queue<string> data){
        commandLines = data;
    }

    // ANCHOR SaveFunctions
    public List<string> Get_txtScripts()
    {
        return txtScripts;
    }
    public Queue<string> Get_commandLines(){
        foreach(var cmd in commandLines){
            branchCache.Enqueue(cmd);
        }
        return branchCache;
    }
    
}
