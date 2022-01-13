using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChoiceManager : MonoBehaviour
{
    // ChoiceCanvas(프리팹)에 할당

    // ANCHOR top
    
    // 출처
    // https://keidy.tistory.com/226
    // 해당 블로그 동영상을 보고 참고하여 제작하였음.

    public static ChoiceManager instance;

    // 싱글톤
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // 질문지, 답변지 텍스트 저장용
    private string question; // 질문지
    private List<string> answerList; // 선택지 리스트

    public GameObject go; // 평소에 비활성화 시킬 목적으로 선언, setActive로 활성

    public Text questionText; // 질문지 텍스트
    public Text[] answerText; // 선택지 텍스트 배열
    public GameObject[] answerPanel; // 선택지 개수에 맞게 활성화 하기 위함. 질문지는 없음

    public Animator anim; // 창 열리고 닫히는 애니메이션 동작

    public bool choiceIng; // 선택지 대기, 다음 대화로 넘어가지 않게
    //private bool keyInput; // 키처리 활성화, 비활성화, 키 입력이 없기때문에 제거하였음.

    private int count; // 배열의 크기
    private int result; // 선택지 선택 결과, 0~4

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // 초기화
    void Start()
    {
        result = 0;
        answerList = new List<string>();
        for (int i = 0; i < answerText.Length; i++)
        {
            answerText[i].text = "";
            answerPanel[i].SetActive(false);
        }
        questionText.text = "";
    }
    
    // ANCHOR Utils (Set_question, Add_answerList, GetResult, SetResult)
    /// <summary>
    /// 데이터 초기화 관련 함수들
    /// Scripts/HappeningOccur/GetNextHappening.cs에서 호출해서
    /// 현재 클래스의 값들을 초기화 할 수 있도록 함.
    /// </summary>
    public void Set_question(string line){
        question = line;
    }
    public string Get_question(){
        return question;
    }
    public void Add_answerList(string line){
        answerList.Add(line);
    }
    public void Clear_answerList(){
        answerList.Clear();
    }
    public List<string> Get_answerList(){
        return answerList;
    }
    public int GetResult(){
        return result;
    }
    public void SetResult(int what){
        result = what;
    }


    // ANCHOR ShowChoice
    /// <summary>
    /// 질문지, 선택지를 개수에 맞춰서 보여주는 함수
    /// 선택지 개수에 맞게 활성화 하는 기능,
    /// 애니메이션 실행,
    /// 각종 데이터 초기화 (result, choiceIng)를 담당
    /// </summary>
    public void ShowChoice()
    {
        go.SetActive(true); // 맨처음에
        result = 0;
        choiceIng = true;
        for (int i = 0; i < answerList.Count; i++)
        {
            answerPanel[i].SetActive(true);
            count = i;
        }

        anim.SetBool("Appear", true);
        //Selection(); // result 초기화에 따른 변경
        StartCoroutine(ChoiceCoroutine());
    }

    // ANCHOR ExitChoice
    /// <summary>
    /// 선택지를 눌렀을 때 창 닫힘 처리 함수
    /// 애니메이션 동작,
    /// 선택지 비활성화,
    /// 리스트 클리어 등의 기능을 함.
    /// </summary>
    public void ExitChoice(){
        anim.SetBool("Appear",false);
        for(int i=0;i<=count;i++){
            answerText[i].text = "";
            answerPanel[i].SetActive(false);
        }
        choiceIng = false;
        questionText.text = "";
        answerList.Clear();

        goSetActiveFalse(); // 맨 마지막에
    }

    /// <summary>
    /// 닫히는 애니메이션이 보이고 난 다음에
    /// 창이 비활성화 되도록
    /// 시간 간격을 주기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator goSetActiveFalse(){
        yield return new WaitForSeconds(0.2f);
        go.SetActive(false);
    }


    // ANCHOR ChoiceCoroutine
    /// <summary>
    /// ShowChoice 함수의 끝에 실행되는 함수로
    /// 창 열림 애니메이션 후에
    /// 질문지와 선택지 글자를 타이핑하는 효과를 주기 위한
    /// 코루틴이다.
    /// </summary>
    /// <returns></returns>
    IEnumerator ChoiceCoroutine()
    {
        yield return new WaitForSeconds(0.2f); // 창 열림/닫힘 애니메이션 지연시간

        StartCoroutine(TypingQuestion());
        StartCoroutine(TypingAnswer0());
        if(count >= 1){
            StartCoroutine(TypingAnswer1());
        }
        if(count >= 2){
            StartCoroutine(TypingAnswer2());
        }
        if(count >= 3){
            StartCoroutine(TypingAnswer3());
        }
        if(count >= 4){
            StartCoroutine(TypingAnswer4());
        }
        yield return new WaitForSeconds(0.5f);

        //keyInput = true;
    }
    // 코루틴 하나로 다돌리면 문제 생긴다고 함.
    // 그래서 여러개로 나누어서 한번에 실행?한다고 함
    IEnumerator TypingQuestion()
    {
        for (int i = 0; i < question.Length; i++)
        {
            questionText.text += question[i];
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer0(int type = 0)
    {
        yield return new WaitForSeconds(0.4f + type * 0.1f);
        for (int i = 0; i < answerList[type].Length; i++)
        {
            answerText[type].text += answerList[type][i];
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer1(int type = 1)
    {
        yield return new WaitForSeconds(0.4f + type * 0.1f);
        for (int i = 0; i < answerList[type].Length; i++)
        {
            answerText[type].text += answerList[type][i];
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer2(int type = 2)
    {
        yield return new WaitForSeconds(0.4f + type * 0.1f);
        for (int i = 0; i < answerList[type].Length; i++)
        {
            answerText[type].text += answerList[type][i];
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer3(int type = 3)
    {
        yield return new WaitForSeconds(0.4f + type * 0.1f);
        for (int i = 0; i < answerList[type].Length; i++)
        {
            answerText[type].text += answerList[type][i];
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer4(int type = 4)
    {
        yield return new WaitForSeconds(0.4f + type * 0.1f);
        for (int i = 0; i < answerList[type].Length; i++)
        {
            answerText[type].text += answerList[type][i];
            yield return waitTime;
        }
    }


    /// <summary>
    /// 원래는 패널을 사용하고 키보드 조작을 통해 선택을 했는데 (블로그에서)
    /// 그 때 패널을 선택하기 위한 키보드 조작 관련 함수
    /// 지금은 버튼을 마우스를 통해 클릭하기 때문에 제거하였음.
    /// </summary>
    // private void Update() {
    //     if(keyInput){
    //         if(Input.GetKeyDown(KeyCode.UpArrow)){
    //             if(result > 0)result--;
    //             else result = 0;
    //             //Selection();
    //         }
    //         else if(Input.GetKeyDown(KeyCode.DownArrow)){
    //             if(result < count) result++;
    //             else result = count;
    //             //Selection();
    //         }
    //         else if(Input.GetKeyDown(KeyCode.Z)){
    //             keyInput = false;
    //             ExitChoice();
    //         }
    //     }
    // }


    /// <summary>
    /// 원래는 패널을 사용하고 키보드 조작을 통해 선택을 했는데 (블로그에서)
    /// 그 때 현재 보고있는 패널의 색을 변경하기 위해서 사용되었던 함수.
    /// 지금은 버튼을 사용하기 때문에 제거하였음. (버튼에는 원래 이 함수의 기능이 있음)
    /// </summary>
    // public void Selection(){
    //     Color color = answerPanel[0].GetComponent<Image>().color;
    //     color.a = 0.75f;
    //     for(int i=0;i<=count;i++){
    //         answerPanel[i].GetComponent<Image>().color = color;
    //     }
    //     color.a = 1f;
    //     answerPanel[result].GetComponent<Image>().color = color;
    // }


    // ANCHOR CatchClick
    /// <summary>
    /// 각 선택지 버튼을 클릭했을 때, OnClick을 통해
    /// 현재 클릭한 버튼의 index를 전달받아 result를 그 값으로 변경
    /// </summary>
    /// <param name="what">선택지 버튼 idx</param>
    public void CatchClick(int what){
        result = what;
        //keyInput = false;
        ExitChoice();
    }
}
