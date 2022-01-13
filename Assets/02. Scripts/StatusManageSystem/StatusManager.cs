using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class StatusManager : MonoBehaviour
{

    public static StatusManager instance;
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

    // ANCHOR Status Definition
    private int networking; // 인맥
    private int eloquence; // 언변
    private int reputation; // 평판
    private int funds; // 자금
    private int approval_rating; // 지지율


    // ANCHOR SaveData
    /// <summary>
    /// 인맥, 언변, 평판, 자금, 지지율을
    /// 저장, 로드 스크립트에 전달
    /// </summary>
    /// <returns></returns>
    public Tuple<int,int,int,int,int> SaveData(){
        return new Tuple<int, int, int, int, int>(
            networking,eloquence,reputation,funds,approval_rating
        );
    }
    
    // LoadData
    /// <summary>
    /// 인맥, 언변, 평판, 자금, 지지율을
    /// 저장, 로드 스크립트로부터 받아옴
    /// </summary>
    public void LoadData(){

    }

    
}
