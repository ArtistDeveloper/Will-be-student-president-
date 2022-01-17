using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GetNextHappening : MonoBehaviour
{
    // GamePlayScene(ChoiWonYoung)->MainScreenCanvas->Debug->DebugButtonPanel->NextDay에 할당


    // true이면 다음날 누르자마자 첫번째 대화가 출력됨. false면 Next 눌러야 첫번째 대화가 출력됨.
    private bool fastNextDialogPrint = false;

    // ANCHOR top
    public string defaultFolderPath; // 이벤트 대화들을 저장하는 파일들의 기본 폴더 경로

    private List<string> txtScripts; // 이벤트 대화들을 저장하는 리스트
    private int txtScriptsIndex; // 현재 몇번째 대화를 보고 있는지
    private List<string> branchFilePath;
    bool choiceIng, branchFlag;

    bool nextDayFlag = false;

    public Text dialogText; // 게임 화면 맨 아래 텍스트 창
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    public GameObject nextDayWarning;



    private void Start()
    {
        txtScripts = new List<string>();
        branchFilePath = new List<string>() { "", "", "", "", "" };
        choiceIng = false;
        branchFlag = false;
    }


    // ANCHOR GetNext
    /// <summary>
    /// 다음 이벤트를 읽어옴.
    /// </summary>
    public void GetNext()
    {
        if (choiceIng) return;
        txtScripts.Clear(); // 대화 로그 초기화
        txtScriptsIndex = 0; // 대화 로그 번호 초기화
        branchFlag = false;


        HappeningUtils.instance.DebugPrintHappening__(
            HappeningUtils.instance.GetPresentHappening__());

        ReadHappeningScripts(
            MakeFilePath(HappeningUtils.instance.GetPresentHappening__().Item2.ToString())
        );
        if (fastNextDialogPrint)
        {
            PrintNextScripts();
        }

        HappeningUtils.instance.IncreaseHappeningIdx();
    }


    // ANCHOR PrintNextScripts
    /// <summary>
    /// 다음 대화를 출력
    /// </summary>
    public void PrintNextScripts()
    {
        if (txtScripts == null) return;
        if (choiceIng) return; // 선택하고 있을 때는 대화를 못움직이게
        if (txtScriptsIndex < txtScripts.Count)
        {
            Debug.Log("" + txtScriptsIndex + "번째 대화 | " + txtScripts[txtScriptsIndex]);
            StartCoroutine(TypingDialogCoroutine(txtScripts[txtScriptsIndex]));
            txtScriptsIndex++;
            nextDayFlag = false;
        }
        else
        {
            // 선택지가 있으면
            if (branchFlag)
            {
                // 분기플래그를 끄고
                branchFlag = false;
                // 선택지 선택까지 기다린 후 선택지에 맞는 대화파일을 출력하는 코루틴을 호출
                StartCoroutine(BranchWaitCoroutine());
            }
            else
            {
                // 대화를 끝까지 읽은 후 선택지가 없을 때,
                // 다음 대화를 2번 누르면 다음날로 넘어갈 수 있는 기능
                if (nextDayFlag)
                {
                    GetNext();
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
    }


    // ANCHOR PrintBackScripts
    /// <summary>
    /// 이전 대화를 출력
    /// </summary>
    public void PrintBackScripts()
    {
        if (txtScripts == null) return;
        if (choiceIng) return; // 선택하고 있을 때는 대화를 못움직이게
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

    private IEnumerator TypingDialogCoroutine(string line)
    {
        dialogText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += line[i];
            yield return waitTime;
        }
    }



    // ANCHOR ReadHappeningScripts
    /// <summary>
    /// 읽을 이벤트 파일 이름을 받아서 해당 대화 스크립트를 읽고,
    /// txtScripts의 리스트 스트링 변수에 저장한다.
    /// 
    /// 만약 파일을 읽다가 BRANCH라는 명령어가 있으면
    /// 질문지에 적힐 텍스트
    /// 선택지 개수
    /// 선택지에 적힐 텍스트 * 선택지 개수
    /// 선택지 선택 후 이동할 텍스트파일명 * 선택지 개수
    /// 를 읽어 각각
    /// ChoiceSystem/Choice.cs의 string question
    /// GetNextHappening.cs의 count
    /// ChoiceSystem/Choice.cs의 List[string] answerList
    /// GetNextHappening.cs의 branchFilePath
    /// 에 저장한다.
    /// </summary>
    /// <param name="happening">
    /// GetNext, BranchWaitCoroutine 함수로부터 받은 이벤트 대화 파일 이름
    /// </param>
    private void ReadHappeningScripts(string filePath)
    {
        // 아래 주석 없애면 선택지 부분 넘어가면 선택지 이전 대화를 볼 수 없음.
        //txtScripts.Clear();
        //txtScriptsIndex = 0;
        try
        {
            StreamReader happeningsTxtScripts = new StreamReader(
                new FileStream(filePath, FileMode.Open));
            string line = "";
            while (happeningsTxtScripts.EndOfStream != true)
            {
                line = happeningsTxtScripts.ReadLine();
                // 주석 거르기
                if (line.Length == 0) continue;
                if (line.Length <= 2 || (line[0] == '/' && line[1] == '/')) continue;
                if (line == "BRANCH")
                {
                    branchFlag = true;

                    // 질문지 제목
                    ChoiceManager.instance.Set_question(happeningsTxtScripts.ReadLine());

                    // 선택지 개수
                    int count = int.Parse(happeningsTxtScripts.ReadLine());

                    // 선택지 텍스트 채우기
                    for (int i = 0; i < count; i++)
                    {
                        ChoiceManager.instance.Add_answerList(happeningsTxtScripts.ReadLine());
                    }

                    // 분기 파일 이름 채우기
                    for (int i = 0; i < count; i++)
                    {
                        branchFilePath[i] = happeningsTxtScripts.ReadLine();
                    }
                }
                else
                {
                    txtScripts.Add(line);
                    branchFlag = false;
                }
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
        ReadHappeningScripts(
            MakeFilePath(branchFilePath[ChoiceManager.instance.GetResult()])
        );
        yield return new WaitUntil(() => !choiceIng);
        if (fastNextDialogPrint)
        {
            PrintNextScripts();
        }
    }

    // ANCHOR MakeFilePath
    /// <summary>
    /// 대화 스크립트가 있는 파일 경로를 만드는 함수
    /// id = 114에 해당하는 스크립트들은 모두
    /// TextData/HappeningTxtScripts/114/ 폴더 안에 있으며
    /// 해당 폴더 안의 파일들은 114, 114_*, 114_*_*_* 등의 형태를 가진다.
    /// 114_ 뒤의 형식은 상관없지만, 114 또는 114_의 형태를 통해 114라는 ID를 분리할 수 있어야 함.
    /// </summary>
    /// <param name="key">대화 파일 이름</param>
    /// <returns>대화 파일 경로</returns>
    private string MakeFilePath(string key)
    {
        string[] tmp = key.Split('_');
        return (defaultFolderPath + '/' + tmp[0] + '/' + key + ".txt");
    }



    // ANCHOR InitFunctions
    public void Set_txtScripts(List<string> data)
    {
        txtScripts.Clear();
        foreach (var line in data)
        {
            txtScripts.Add(line);
        }
    }
    public void Set_branchFlag(bool data)
    {
        branchFlag = data;
    }
    public void Set_question(string data)
    {
        ChoiceManager.instance.Set_question(data);
    }
    public void Set_answerList(List<string> data)
    {
        ChoiceManager.instance.Clear_answerList();
        foreach (var line in data)
        {
            ChoiceManager.instance.Add_answerList(line);
        }
    }
    public void Set_branchFilePath(List<string> data)
    {
        for (int i = 0; i < 5; i++)
        {
            branchFilePath[i] = data[i];
        }
    }

    // ANCHOR SaveFunctions
    public List<string> Get_txtScripts()
    {
        return txtScripts;
    }
    public bool Get_branchFlag()
    {
        return branchFlag;
    }
    public string Get_question()
    {
        return ChoiceManager.instance.Get_question();
    }
    public List<string> Get_answerList()
    {
        return ChoiceManager.instance.Get_answerList();
    }
    public List<string> Get_branchFilePath()
    {
        return branchFilePath;
    }
}