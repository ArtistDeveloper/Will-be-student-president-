using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    public static StatusManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // ANCHOR 스탯 수치 변동값 정의
            alphaMulti = Math.Ceiling((WHOLE_STUDENTS_NUMBER * 1.0f) / (MAX_STATUS * 1.0f));
            networkingContribution *= alphaMulti * 0.01f;
            eloquenceContribution *= alphaMulti * 0.01f;
            reputationContribution *= alphaMulti * 0.01f;
            moneyContribution *= alphaMulti * 0.01f;
            settingFlag = false;


            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // ANCHOR 변수들 정의
    public int MAX_STATUS;
    public int WHOLE_STUDENTS_NUMBER;
    public double networkingContribution;
    public double eloquenceContribution;
    public double reputationContribution;
    public double moneyContribution;
    private double alphaMulti;


    private int networking; // 인맥
    private int eloquence; // 언변
    private int reputation; // 평판
    private int money; // 자금
    private int approvalRating; // 지지율

    public Text networkingText;
    public Text eloquenceText;
    public Text reputationText;
    public Text moneyText;
    public Text approvalRatingText;


    public bool settingFlag;



    // ANCHOR 텍스트 컴포넌트 연결
    public void SetTextComponent(Text n, Text e, Text r, Text m, Text a){
        networkingText = n;
        eloquenceText = e;
        reputationText = r;
        moneyText = m;
        approvalRatingText = a;
        ApplyStatusToText();
    }

    // ANCHOR 스탯 변경 (현재 = 현재 + 변동값)
    /// <summary>
    /// 스탯 변경 (현재 = 현재 + 변동값)
    /// </summary>
    /// <param name="networking"></param>
    /// <param name="eloquence"></param>
    /// <param name="reputation"></param>
    /// <param name="money"></param>
    public void ChangeStatus(int networking, int eloquence, int reputation, int money)
    {
        SetStatus(this.networking + networking, this.eloquence + eloquence, this.reputation + reputation, this.money + money);
    }

    // ANCHOR 스텟 변경 (현재 = 변동값)
    public void SetStatus(int n, int e, int r, int m)
    {
        networking = Math.Min(n, MAX_STATUS);
        eloquence = Math.Min(e, MAX_STATUS);
        reputation = Math.Min(r, MAX_STATUS);
        money = Math.Min(m, MAX_STATUS);
        // 지지율 계산
        approvalRating = Math.Min((int)(
            networking * networkingContribution + eloquence * eloquenceContribution + reputation * reputationContribution + money * moneyContribution),
            WHOLE_STUDENTS_NUMBER
        );

        Debug.Log("지지율 : " + approvalRating);
        ApplyStatusToText();
    }


    // ANCHOR 스텟 값 텍스트로 적용
    public void ApplyStatusToText()
    {
        networkingText.text = this.networking.ToString();
        eloquenceText.text = this.eloquence.ToString();
        reputationText.text = this.reputation.ToString();
        moneyText.text = this.money.ToString();
        approvalRatingText.text = approvalRating.ToString();
    }


    // ANCHOR 데이터 저장용 함수
    /// <summary>
    /// 인맥, 언변, 평판, 자금, 지지율을
    /// 저장, 로드 스크립트에 전달
    /// </summary>
    /// <returns></returns>
    public Tuple<int,int,int,int> SaveStatus(){
        return new Tuple<int, int, int, int>(
            networking,eloquence,reputation,money
        );
    }
    

    // ANCHOR 각 스텟 가져오기
    public int GetNetworking(){
        return networking;
    }
    public int GetEloquence(){
        return eloquence;
    }
    public int GetReputation(){
        return reputation;
    }
    public int GetMoney(){
        return money;
    }
    public int GetApprovalRating(){
        approvalRating = Math.Min((int)(
            networking * networkingContribution + eloquence * eloquenceContribution + reputation * reputationContribution + money * moneyContribution),
            WHOLE_STUDENTS_NUMBER
        );
        return approvalRating;
    }













    /*

    // -------------------------------------------------------------------------------------------
    // -----------------------------       지워야되는 내용       ---------------------------------
    // -------------------------------------------------------------------------------------------
    public void SetStatusValue(int networking, int eloquence, int reputation, int money)
    {
        this.networking = Math.Min(networking, MAX_STATUS);
        this.eloquence = Math.Min(eloquence, MAX_STATUS);
        this.reputation = Math.Min(reputation, MAX_STATUS);
        this.money = Math.Min(money, MAX_STATUS);
        // 지지율 계산
        approvalRating = Math.Min((int)(
            networking * networkingContribution + eloquence * eloquenceContribution + reputation * reputationContribution + money * moneyContribution),
            WHOLE_STUDENTS_NUMBER
        );
    }
    // LoadData
    /// <summary>
    /// 인맥, 언변, 평판, 자금, 지지율을
    /// 저장, 로드 스크립트로부터 받아옴
    /// </summary>
    public void LoadStatus(Tuple<int,int,int,int> data){
        Debug.Log("asdf");
        SetStatusValue(data.Item1,data.Item2,data.Item3,data.Item4);
        //ApplyStatusToText();
    }
    // -------------------------------------------------------------------------------------------
    */
}