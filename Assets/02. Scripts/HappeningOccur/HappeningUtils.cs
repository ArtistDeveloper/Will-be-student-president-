using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HappeningUtils : MonoBehaviour
{
    // MainScreen->HappeningUtils에 할당

    // ANCHOR Top
    // 싱글톤 패턴
    public static HappeningUtils instance = null;


    // HappeningData.txt 파일 경로 저장 변수
    public string happeningDataFilePath; 

    // happening 데이터
    /// <summary>
    /// typeIdList : HappeningData.txt의 type과 ID를 연결하는 리스트
    /// happeningTitles : HappeningData.txt의 이벤트 제목과 ID를 연결하는 딕셔너리
    /// happeningOccurType : HappeningData.txt의 ID와 제외요일, 발생확률, type를 연결하는 딕셔너리
    /// happeningOccurCntConst : HappeningData.txt의 ID와 발생 횟수를 연결하는 딕셔너리. 아래 딕셔너리에 복제해서 사용해야함
    /// happeningOccurCnt : HappeningData.txt의 ID와 발생 횟수를 연결하는 딕셔너리. 값이 자주 바뀌기 때문에 따로 만듦
    /// happeningOccurRange : HappeningData.txt의 ID와 발생날짜 시작~끝 리스트를 연결하는 딕셔너리
    /// </summary>
    private List<Tuple<int, int>> typeIdList;
    private Dictionary<int, string> happeningTitles;
    private Dictionary<int, Tuple<int, int, int>> happeningOccurType; // id : exc date | occur % | type
    private Dictionary<int, int> happeningOccurCntConst; // 발생 횟수 저장용
    private Dictionary<int, int> happeningOccurCnt; // 발생 횟수 계산용
    private Dictionary<int, List<Tuple<int, int>>> happeningOccurRange; // 발생 범위 딕셔너리



    // 나중에 할당할 이벤트들
    /// <summary>
    /// etcHappeningsIdx : HappeningData.txt의 type 10에 해당하는 이벤트들의 ID 리스트
    /// waitingHappeningsIdx : HappeningData.txt의 type 11에 해당하는 이벤트들의 ID 리스트
    /// examLinkedHappeningsIdx : HappeningData.txt의 type 12에 해당하는 이벤트들의 ID 리스트
    /// vacationHappeningsIdx : HappeningData.txt의 type 13에 해당하는 이벤트들의 ID 리스트
    /// </summary>
    private List<int> etcHappeningsIdx, waitingHappeningsIdx, examLinkedHappeningsIdx, vacationHappeningsIdx;



    // 이벤트 순서
    private List<Tuple<int, int>> happeningStream; // 왼쪽은 날짜(int), 오른쪽은 데이터베이스 id
    // 현재 날짜, happeningStream next값 가져오기 위한 변수(present~~)
    private int dateNow, presentHappeningIdx;

    // 기타 데이터들
    private int[] Month = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    public enum dayOfTheWeek { Mon, Tue, Wed, Thu, Fri, Sep, Sun };
    private List<Tuple<int, int>> Vacation;
    private System.Random random;
    public String startDate, endDate;
    private int startDate_, endDate_;

    public String summerVactionStart, summerVactionEnd;
    public String winterVactionStart, winterVactionEnd;
    public String springVactionStart, springVactionEnd;
    private int smvs, smve, wtvs, wtve, spvs, spve; // int로 변환된 0년째 방학 날짜들

    public bool settingFlag;



    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Util Class created successfully");
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitSettings();
        }
        else
        {
            Debug.Log("Util Class alread exists!");
            Destroy(this.gameObject);
        }
    }

    // ANCHOR InitSettings
    /// <summary>
    /// 변수 초기화
    /// 
    /// 맨 처음에 단 한번만 실행됨
    /// 게임에 필요한 여러 세팅들을 초기화하는 함수
    /// </summary>
    private void InitSettings()
    {
        startDate_ = StringToInt(startDate);
        endDate_ = StringToInt(endDate);
        random = new System.Random();
        dateNow = startDate_;


        typeIdList = new List<Tuple<int, int>>();
        happeningTitles = new Dictionary<int, string>();
        happeningOccurType = new Dictionary<int, Tuple<int, int, int>>();
        happeningOccurCntConst = new Dictionary<int, int>();
        happeningOccurRange = new Dictionary<int, List<Tuple<int, int>>>();


        etcHappeningsIdx = new List<int>();
        waitingHappeningsIdx = new List<int>();
        examLinkedHappeningsIdx = new List<int>();
        vacationHappeningsIdx = new List<int>();

        // 방학 기간 설정
        Vacation = new List<Tuple<int, int>>();
        (smvs, smve) = (StringToInt(summerVactionStart), StringToInt(summerVactionEnd));
        (wtvs, wtve) = (StringToInt(winterVactionStart), StringToInt(winterVactionEnd));
        (spvs, spve) = (StringToInt(springVactionStart), StringToInt(springVactionEnd));
        for (int i = 0; i <= (endDate_ - startDate_) / 365 + 1; i++)
        {
            Vacation.Add(new Tuple<int, int>(smvs + i * 365, smve + i * 365));
            Vacation.Add(new Tuple<int, int>(wtvs + i * 365, wtve + i * 365));
            Vacation.Add(new Tuple<int, int>(spvs + i * 365, spve + i * 365));
        }

        ReadData();
    }


    // ANCHOR ReadData
    /// <summary>
    /// 이벤트 데이터 읽기
    /// 
    /// Assets/TextData/HappeningData.txt 파일을 읽어서
    /// 각 딕셔너리, 리스트 등에 데이터를 저장함.
    /// 맨 처음에 단 한번만 실행됨
    /// </summary>
    private void ReadData()
    {
        StreamReader happeningsTxt = new StreamReader(new FileStream(happeningDataFilePath, FileMode.Open));
        while (happeningsTxt.EndOfStream != true)
        {
            string line = happeningsTxt.ReadLine();
            if (line.Length <= 2 || (line[0] == '/' && line[1] == '/')) continue;
            if (line.Length == 0) continue;
            string[] processedLine = line.Split('\t');
            PushData(ref processedLine);
        }
        happeningsTxt.Close();
    }

    // 파싱 함수
    /// <summary>
    /// ReadData로부터 Assets/TextData/HappeningData.txt의 각 문자열을 받고,
    /// 그 문자열들을 파싱하여 적절하게 데이터를 분배하는 함수
    /// </summary>
    /// <param name="line">
    /// Assets/TextData/HappeningData.txt으로 부터 읽은 문자열 한줄
    /// </param>
    private void PushData(ref string[] line)
    {
        // line[0] : id,    line[1] : type, line[2] : exclude date
        // line[3] : occur cnt, line[4] : occur%,   line[5] : string
        // line[6] : date range cnt
        int id = Convert.ToInt32(line[0]), type = Convert.ToInt32(line[1]);

        // 타입에 따른 저장 위치 예외 처리
        switch (type)
        {
            case 10: // 기타 이벤트
                etcHappeningsIdx.Add(id);
                break;
            case 11: // 예외처리용 대체 이벤트
                waitingHappeningsIdx.Add(id);
                return;
            case 12: // 시험 연계 이벤트
                examLinkedHappeningsIdx.Add(id);
                break;
            case 13: // 방학 이벤트
                vacationHappeningsIdx.Add(id);
                break;
            default:
                typeIdList.Add(new Tuple<int, int>(type, id));
                break;
        }

        // 이벤트 제목 저장
        happeningTitles.Add(id, line[5]); // id <-> title


        int excludeDate = 0, occurCnt, occurPercent, dateRangeCnt;
        string rev = "";
        for (int i = line[2].Length - 1; i >= 0; i--)
        {
            rev += line[2][i];
        }
        excludeDate = Convert.ToInt32(rev, 2);
        occurCnt = Convert.ToInt32(line[3]);
        occurPercent = Convert.ToInt32(line[4]);

        // id <-> 거를요일, 발생확률, 타입
        happeningOccurType.Add(id, new Tuple<int, int, int>(
            excludeDate, occurPercent, type));
        happeningOccurCntConst.Add(id, occurCnt); // 발생횟수

        dateRangeCnt = Convert.ToInt32(line[6]);
        List<Tuple<int, int>> dateRangeList = new List<Tuple<int, int>>();
        if (dateRangeCnt == 0) // 주어진 날짜 범위가 없으면 전체 범위로
        {
            dateRangeList.Add(new Tuple<int, int>(0, endDate_));
        }
        for (int i = 0; i < dateRangeCnt; i++)
        {
            string[] im = line[7 + i].Split('/');
            if (im.Count() == 1) // 날짜에 /가 없으면 방학 type임
            {
                int vacationIdx = Convert.ToInt32(line[7 + i]);
                while (vacationIdx < Vacation.Count)
                {
                    dateRangeList.Add(Vacation[vacationIdx]);
                    vacationIdx += 3;
                }
            }
            else // y/m/d 형식의 날짜인 경우
            {
                // 2개를 읽어야됨
                dateRangeList.Add(new Tuple<int, int>(
                    StringToInt(line[7 + i]), StringToInt(line[8 + i])));
                i++;
                dateRangeCnt++;
            }
        }
        happeningOccurRange.Add(id, dateRangeList);
    }


    // ANCHOR MakeNewProgress
    /// <summary>
    /// 이벤트 진행 순서 새로 만들기
    /// </summary>
    public void MakeNewProgress()
    {
        //happeningStream.Add(1);
        happeningStream = new List<Tuple<int, int>>();
        happeningOccurCnt = new Dictionary<int, int>(happeningOccurCntConst); // 값 변경되서 복제 딕셔너리를 만들어야 함
        presentHappeningIdx = 0; // happeningStream 가리키는 인덱스를 0으로 초기화
        Debug.Log("현재이벤트번호초기화");
        Debug.Log(presentHappeningIdx);

        // 고정 이벤트들 미리 할당하기
        typeIdList.Sort();
        for (int i = 0; i < typeIdList.Count; i++)
        {
            int id = typeIdList[i].Item2, type = typeIdList[i].Item1;
            List<Tuple<int, int>> dates = GetDates(id, type);
            foreach (var date in dates)
            {
                happeningStream.Add(date);
            }
        }
        happeningStream = happeningStream.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();

        // 이벤트간 평균 간격 계산
        int happeningBetweenTerm = (endDate_ - startDate_ - happeningStream.Count()), divIm = 0;
        foreach (var ID in etcHappeningsIdx)
        {
            divIm += happeningOccurCnt[ID];
        }
        foreach (var ID in vacationHappeningsIdx)
        {
            divIm += happeningOccurCnt[ID];
        }
        happeningBetweenTerm = Math.Max(happeningBetweenTerm / divIm, 1);
        //Debug.Log("이벤트간 평균 간격 : " + happeningBetweenTerm.ToString());

        int dateNow = startDate_, beforeIdx = -1;
        for (int i = 0, hpnSize = happeningStream.Count(); i < hpnSize; i++)
        {
            while (dateNow < happeningStream[i].Item1)
            {
                // 이벤트 평균 간격씩 날짜 증가시키면서 이벤트 넣기
                dateNow += happeningBetweenTerm;
                if (dateNow >= happeningStream[i].Item1) break; // 다음 고정 이벤트 날짜를 벗어나면 탈출

                int ID;
                if (IsVacation(dateNow) == true)
                {
                    ID = GetRandomHappening(vacationHappeningsIdx, dateNow, beforeIdx);
                    if (ID == -1 || ID == 0) continue;
                }
                else
                {
                    ID = GetRandomHappening(etcHappeningsIdx, dateNow, beforeIdx);
                    if (ID == -1)
                    {
                        ID = waitingHappeningsIdx[random.Next(0, waitingHappeningsIdx.Count())];
                        while (ID == beforeIdx)
                        {
                            ID = waitingHappeningsIdx[random.Next(0, waitingHappeningsIdx.Count())];
                        }
                    }
                    if (ID == 0) continue;
                }
                happeningStream.Add(new Tuple<int, int>(dateNow, ID));
                beforeIdx = ID;
            }
            dateNow = happeningStream[i].Item1;
        }
        happeningStream = happeningStream.OrderBy(x => x.Item1).ThenBy(x => x.Item2).ToList();


        // 로딩 다 됐는지 확인용 변수
        settingFlag = true;
    }







    // ANCHOR Util Functions

    /// <summary>
    /// 현재 presentHappeningIdx를 반환함
    /// </summary>
    /// <returns></returns>
    public int GetPresentHappeningIdx()
    {
        return presentHappeningIdx;
    }
    public void SetPresentHappeningIdx(int savedPresentHappeningIdx)
    {
        presentHappeningIdx = savedPresentHappeningIdx;
    }

    /// <summary>
    /// 이벤트 발생 순서 얻기
    /// 
    /// 이벤트 [발생일,이벤트키] 튜플 리스트를
    /// 반환하는 함수
    /// </summary>
    /// <returns>
    /// 리스트[튜플[발생일,이벤트키]]
    /// </returns>
    public List<Tuple<int, int>> GetHappeningStream()
    {
        return new List<Tuple<int, int>>(happeningStream);
    }

    public void SetHappeningStream(List<Tuple<int, int>> savedHappeningStream)
    {
        happeningStream = savedHappeningStream;
        settingFlag = true;
    }


    /// <summary>
    /// presentHappeningIdx의 값을 바꿈
    /// </summary>
    /// <param name="change">바꿀 값. 기본 1</param>
    public void IncreaseHappeningIdx(int change = 1)
    {
        presentHappeningIdx += change;
    }


    // ANCHOR GetNextHappening__
    /// <summary>
    /// 다음 이벤트을 가져오는 함수
    /// happeningStream의 presentHappeningIdx번째 튜플을 반환함
    /// </summary>
    /// <returns>
    /// Tuple[발생날짜,이벤트key]
    /// </returns>
    public Tuple<int, int> GetNextHappening__()
    {
        if (happeningStream.Count <= presentHappeningIdx)
        {
            return new Tuple<int, int>(-1, -1);
        }
        return happeningStream[presentHappeningIdx++];
    }
    public Tuple<int, int> GetPresentHappening__()
    {
        if (happeningStream.Count <= presentHappeningIdx)
        {
            return new Tuple<int, int>(-1, -1);
        }
        return happeningStream[presentHappeningIdx];
    }
    public void IncreasePresentHappeningIdx(int increase = 1)
    {
        presentHappeningIdx += increase;
    }

    // ANCHOR DebugPrintHappening__
    /// <summary>
    /// 이벤트 디버그 출력
    /// </summary>
    /// <param name="hpng">
    /// happeningStream의 각 요소
    /// </param>
    public void DebugPrintHappening__(Tuple<int, int> hpng)
    {
        if (hpng.Item2 == -1)
        {
            Debug.Log("No Happenings");
        }
        else
        {
            /*Debug.Log(happeningTitles[hpng.Item2] + " " + hpng.Item1.ToString() + " " +
             IntToDateString(hpng.Item1) + " " + (dayOfTheWeek)DateType(hpng.Item1));*/
            Debug.Log(IntToDateString(hpng.Item1) + " | 요일 : " + (dayOfTheWeek)DateType(hpng.Item1));
            Debug.Log(happeningTitles[hpng.Item2]);
        }
    }

    // ANCHOR IsVacation
    /// <summary>
    /// 방학인지 물어보는 함수
    /// </summary>
    /// <param name="date">현재 날짜</param>
    /// <returns>방학인지 아닌지</returns>
    public bool IsVacation(int date)
    {
        foreach (var range in Vacation)
        {
            if (range.Item1 <= date && date <= range.Item2)
                return true;
        }
        return false;
    }

    private int GetRandomHappening(List<int> happeningList, int dateNow, int beforeIdx)
    {
        // happeningList는 이벤트 맵 key 가지고 있음
        HashSet<int> randomVal = new HashSet<int>();

        // 후보군의 날짜 범위안에 현재 날짜가 포함되는 이벤트들만 뽑기 후보군에 넣는 함수
        for (int i = 0; i < happeningList.Count(); i++)
        {
            if (happeningList[i] == beforeIdx)
            {
                continue;
            }
            if (happeningOccurCnt[happeningList[i]] <= 0)
            {
                continue;
            }
            bool flag = false;
            foreach (var rangeEach in happeningOccurRange[happeningList[i]])
            {
                if (rangeEach.Item1 <= dateNow && dateNow <= rangeEach.Item2)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                randomVal.Add(i);
            }
        }
        if (randomVal.Count == 0) return -1;


        int idx = randomVal.ElementAt(random.Next(randomVal.Count)), ID = happeningList[idx];
        happeningOccurCnt[ID]--;
        if (random.Next(1, 100) > happeningOccurType[ID].Item2)
            return 0;
        return ID;
    }

    private List<Tuple<int, int>> GetDates(int ID, int type)
    {
        List<Tuple<int, int>> ret = new List<Tuple<int, int>>();
        List<Tuple<int, int>> dateRanges = happeningOccurRange[ID];
        int date;
        switch (type)
        {
            case 1:
                for (int i = 0; i <= (endDate_ - startDate_) / 365 + 2; i++)
                {
                    date = GetRandomInRange(dateRanges, null, happeningOccurType[ID].Item1, i * 365);
                    if (date == -1) continue;
                    ret.Add(new Tuple<int, int>(date, ID));
                }
                break;
            case 2:
                date = GetRandomInRange(dateRanges, null, happeningOccurType[ID].Item1);
                if (date == -1) throw new InvalidOperationException("Date Range is not valid");
                ret.Add(Tuple.Create(date, ID));
                break;
            case 3:
                date = GetRandomInRange(dateRanges, null, happeningOccurType[ID].Item1);
                if (date == -1) throw new InvalidOperationException("Date Range is not valid");
                ret.Add(new Tuple<int, int>(date, ID));


                int linkedID = examLinkedHappeningsIdx[random.Next(0, examLinkedHappeningsIdx.Count - 1)];
                if (random.Next(1, 100) <= happeningOccurType[linkedID].Item2)
                {
                    int linkedDate = -1;
                    List<Tuple<int, int>> im = new List<Tuple<int, int>>();
                    im.Add(Tuple.Create(date, date + 10));
                    linkedDate = GetRandomInRange(im, null, happeningOccurType[linkedID].Item1);
                    if (linkedDate == -1) throw new InvalidOperationException("Date Range is not valid");
                    ret.Add(new Tuple<int, int>(linkedDate, linkedID));
                }
                break;
            default:
                throw new InvalidOperationException("Date type error");
        }
        return ret;
    }


    // ANCHOR IntToDate
    /// <summary>
    /// int값을 년/월/일 튜플로 반환
    /// </summary>
    /// <param name="convertDate">날짜(int)</param>
    /// <returns>년/월/일</returns>
    public Tuple<int, int, int> IntToDate(int convertDate)
    {
        int year = 0, month = 1, day = 1;
        convertDate--;
        year = convertDate / 365;
        convertDate %= 365;
        convertDate++;
        for (month = 1; month <= 12; month++)
        {
            if (convertDate <= Month[month])
            {
                day = convertDate;
                break;
            }
            convertDate -= Month[month];
        }
        return (new Tuple<int, int, int>(year, month, day));
    }

    // ANCHOR IntToDateString
    /// <summary>
    /// int값을 string으로 반환
    /// </summary>
    /// <param name="convertDate">날짜 int값</param>
    /// <returns>년/월/일 string</returns>
    public string IntToDateString(int convertDate)
    {
        Tuple<int, int, int> date = IntToDate(convertDate);
        return date.Item1.ToString() + "/" + date.Item2.ToString() + "/" + date.Item3.ToString();
    }

    // ANCHOR DateToInt
    /// <summary>
    /// 년/월/일 튜플을 int값으로 반환
    /// </summary>
    /// <param name="convertDate">년/월/일</param>
    /// <returns>int로 바꾼 날짜 값</returns>
    public int DateToInt(Tuple<int, int, int> convertDate)
    {
        int ret = 0;
        ret += convertDate.Item1 * 365;
        ret += convertDate.Item3;
        for (int i = 1; i < convertDate.Item2; i++) ret += Month[i];
        return ret;
    }

    // ANCHOR StringToDate
    /// <summary>
    /// 년/월/일 문자열을 튜플로 반환
    /// </summary>
    /// <param name="convertDate">년/월/일 문자열</param>
    /// <returns>년/월/일 튜플</returns>
    public Tuple<int, int, int> StringToDate(String convertDate)
    {
        string[] dateList = convertDate.Split('/');
        if (dateList.Count() != 3)
            throw new InvalidOperationException("Date String Format is not valid");
        return new Tuple<int, int, int>(
            Convert.ToInt32(dateList[0]),
            Convert.ToInt32(dateList[1]),
            Convert.ToInt32(dateList[2])
        );
    }

    // ANCHOR StringToInt
    /// <summary>
    /// 년/월/일 문자열을 int값으로 반환
    /// </summary>
    /// <param name="convertDate">년/월/일 문자열</param>
    /// <returns>날짜 int값</returns>
    public int StringToInt(String convertDate)
    {
        return DateToInt(StringToDate(convertDate));
    }

    // ANCHOR DateType
    /// <summary>
    /// 요일 계산기
    /// 0~6중 하나 반환
    /// 반환값에 (dayOfTheWeek) 붙이면 Mon~Sun으로 바꿔줌
    /// </summary>
    /// <param name="convertDate">날짜 int값</param>
    /// <returns>0~6중 하나</returns>
    public int DateType(int convertDate)
    {
        return ((convertDate - 1) % 7);
    }

    // 범위에서 랜덤 뽑기 (요일 범위 정하고, 거를 요일 비트연산으로 넣어서 전달)
    private int GetRandomInRange(List<Tuple<int, int>> rangeList, List<Tuple<int, int>> excludeRangeList = null, int excludeDateType = 0, int offset = 0)
    {
        HashSet<int> randomVal = new HashSet<int>();
        HashSet<int> excludeVal = new HashSet<int>();
        foreach (Tuple<int, int> range in rangeList)
        {
            if (range.Item2 + offset < startDate_) continue;
            for (int i = range.Item1 + offset; i <= range.Item2 + offset && i <= endDate_; i++)
            {
                if (i < startDate_) continue;
                if (((1 << DateType(i)) & excludeDateType) != 0) continue;
                randomVal.Add(i);
            }
        }
        if (excludeRangeList != null)
        {
            foreach (Tuple<int, int> range in excludeRangeList)
            {
                if (range.Item2 + offset < startDate_) continue;
                for (int i = range.Item1 + offset; i <= range.Item2 + offset && i <= endDate_; i++)
                {
                    if (i < startDate_) continue;
                    excludeVal.Add(i);
                }
            }
        }
        foreach (var happening in happeningStream)
        {
            excludeVal.Add(happening.Item1);
        }
        randomVal.ExceptWith(excludeVal);

        if (randomVal.Count == 0)
            return -1;
        return randomVal.ElementAt(random.Next(randomVal.Count));
    }


    // ANCHOR GetPresentHappeningTitle
    public string GetPresentHappeningTitle(){
        return happeningTitles[happeningStream[presentHappeningIdx].Item2];
    }
}