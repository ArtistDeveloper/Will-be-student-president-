using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using DG.Tweening;


public class TestGagueData : MonoBehaviour
{
    public int MAX_STATUS;
    public int networking;
    public int eloquence;
    public int reputation;
    public int money;
    public float networkingContribution;
    public float eloquenceContribution;
    public float reputationContribution;
    public float moneyContribution;

    int stageNum = 0;
    int supportersNum = 0;
    float nowApprovalRating;
    public float NowApprovalRating{
        get{return nowApprovalRating;}
    }

    public int[] GOAL_SUPPORTERS_NUM;
    public int WHOAL_STUDENTS_NUM;

    public void SetApprovalRating()
    {
        supportersNum = Math.Min((int)(
            networking * networkingContribution + eloquence * eloquenceContribution + reputation * reputationContribution + money * moneyContribution),
            GOAL_SUPPORTERS_NUM[stageNum]
        );
        //PrintApprovalRatinge();
	    ChangeGaugeUI();
        Debug.Log(4);
        
    }

    // ANCHOR   GaugeUI관련함수

    public Image gaugeImg;
    float changedApprovalRating;
    float changedFillAmount = 0;
    Tween gaugeTween;

    
    
    public void ChangeGaugeUI()
    {
        nowApprovalRating = (float)supportersNum/GOAL_SUPPORTERS_NUM[stageNum];
        changedApprovalRating = changedFillAmount - (nowApprovalRating);
        PrintApprovalRatinge();
        if(changedApprovalRating == 0)  return;
        else 
        {
            Debug.Log("변경 전 fillAmount");
            Debug.Log(changedFillAmount);
            float targetGaugeFillAmount = (changedFillAmount-changedApprovalRating);
            changedFillAmount = targetGaugeFillAmount;

            Debug.Log(0);
            StartCoroutine(CoUpdateGaugeUI(targetGaugeFillAmount, 0.5f));
            Debug.Log(2);
             Debug.Log("변경 후 fillAmount");
        Debug.Log(changedFillAmount);

           
        }
    }

    public IEnumerator CoUpdateGaugeUI(float targetGaugeFillAmount, float duration) 
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        
        while (duration > 0) {
            duration -= Time.deltaTime;
            gaugeImg.fillAmount = Mathf.Lerp(gaugeImg.fillAmount, targetGaugeFillAmount, Time.deltaTime/duration);
            
            yield return null;

        }
        
    }

    public void NextStage()
    {
        Math.Min(stageNum += 1, 5);

        Debug.Log("스테이지번호");
        Debug.Log(stageNum);
        Debug.Log("목표 지지자 수");
        Debug.Log(GOAL_SUPPORTERS_NUM[stageNum]);

        ChangeGaugeUI();
    }
    //ANCHOR 스탯 증가 함수(버튼)
    
    public void UpNetworking()
    {
        Math.Min(networking += 1, MAX_STATUS);
        PrintStatus();
    }
    public void UpEloquence()
    {
        Math.Min(eloquence += 1, MAX_STATUS);
        PrintStatus();
    }
    public void UpReputation()
    {
        Math.Min(reputation += 1, MAX_STATUS);
        PrintStatus();
    }
     public void UpMoney()
    {
       Math.Min(money += 1, MAX_STATUS);
       PrintStatus();
    }
    //ANCHOR 스탯 감소 함수(버튼)
    public void DownNetworking()
    {
        Math.Max(networking -= 1, 0);
        PrintStatus();
    }
    public void DownEloquence()
    {
        Math.Max(eloquence -= 1, 0);
        PrintStatus();
    }
    public void DownReputation()
    {
        Math.Max(reputation -= 1, 0);
        PrintStatus();
    }
     public void DownMoney()
    {
        Math.Max(money -= 1, 0);
        PrintStatus();
    }
    // ANCHOR 출력관련 함수
    public void PrintStatus()
    {
        Debug.Log("스테이터스 값");
        Debug.Log(networking);
        Debug.Log(eloquence);
        Debug.Log(reputation);
        Debug.Log(money);
    }

    public void PrintApprovalRatinge()
    {
        Debug.Log("스테이지번호");
        Debug.Log(stageNum);
        Debug.Log("목표 지지자 수");
        Debug.Log(GOAL_SUPPORTERS_NUM[stageNum]);
        Debug.Log("지지자 수");
        Debug.Log(supportersNum);
        Debug.Log("지지율");
        Debug.Log(nowApprovalRating);
    }
}
