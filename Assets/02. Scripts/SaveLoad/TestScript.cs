using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // 임시로 저장기능과 로드 기능을 테스트하기 위한 스크립트
    // 해프닝 순서, 해프닝 인덱스가 들어감

    List<Tuple<int, int>> happeningStream = new List<Tuple<int, int>>();  // 해프닝 순서
    int presentHappeningIdx;       // 해프닝 번호

    private void Awake() 
    {
        Tuple<int, int> tuple = new Tuple<int, int> (1,1);
        happeningStream.Add(tuple);
        happeningStream.Add(tuple);
        happeningStream.Add(tuple);
        happeningStream.Add(tuple);
        presentHappeningIdx = 4;
    }

    public List<Tuple<int,int>> GetHappeningStream()
    {
        return happeningStream;
    }
    public int GetPresentHappeningIdx()
    {
        return presentHappeningIdx;
    }
}
