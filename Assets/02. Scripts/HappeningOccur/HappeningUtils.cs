using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

public class HappeningUtils : MonoBehaviour
{
    public static HappeningUtils happeningUtils;

    // happening 데이터
    private List<Tuple<int, int>> typeIdList;
    public Dictionary<int, string> happeningTitles;
    private Dictionary<int, Tuple<int, int, int>> happeningOccurType; // id : exc date | occur % | type
    private Dictionary<int, int> happeningOccurCnt; // 발생 횟수 계산용
    private Dictionary<int, List<Tuple<int, int>>> happeningOccurRange; // 발생 범위 딕셔너리

    // 나중에 할당할 이벤트들
    private List<int> etcHappeningsIdx, waitingHappeningsIdx, examLinkedHappeningsIdx, vacationHappeningsIdx;

    // 이벤트 순서
    public List<Tuple<int, int>> happeningStream; // 왼쪽은 날짜(int), 오른쪽은 데이터베이스 id
    // 현재 날짜, happeningStream next값 가져오기 위한 변수(present~~)
    private int dateNow, presentHappeningIdx;

    // 기타 데이터들
    private int[] Month = new int[] { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    public enum dayOfTheWeek { Mon, Tue, Wed, Thu, Fri, Sep, Sun };
    private List<Tuple<int, int>> Vacation;
    private System.Random random;
    public String startDate, endDate;
    private int startDate_, endDate_;
    //private Tuple<int, int> summerVacRange, winterVacRange, springVacRange;

    public String summerVactionStart, summerVactionEnd;
    public String winterVactionStart, winterVactionEnd;
    public String springVactionStart, springVactionEnd;
    private int smvs, smve, wtvs, wtve, spvs, spve;
    private void Awake()
    {
        if (happeningUtils == null)
        {
            Debug.Log("Util Class created successfully");
            happeningUtils = this;
        }
        else
        {
            Debug.Log("Util Class alread exists!");
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello Utils!");

        startDate_ = StringToInt(startDate);
        endDate_ = StringToInt(endDate);
        random = new System.Random();
        dateNow = startDate_;
        presentHappeningIdx = 0;

        waitingHappeningsIdx = new List<int>();

        // 방학 기간 설정?
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

        typeIdList = new List<Tuple<int, int>>();
        happeningTitles = new Dictionary<int, string>();
        happeningOccurType = new Dictionary<int, Tuple<int, int, int>>();
        happeningOccurCnt = new Dictionary<int, int>();
        happeningOccurRange = new Dictionary<int, List<Tuple<int, int>>>();
        happeningStream = new List<Tuple<int, int>>();
        etcHappeningsIdx = new List<int>();
        examLinkedHappeningsIdx = new List<int>();
        vacationHappeningsIdx = new List<int>();
        ReadData();
        MakeProgress();
        //Debug.Log(IntToDate(StringToInt("0/1/6") + 365)); // -> 1/1/6으로 출력
    }

    public void ReadData()
    {
        StreamReader happeningsTxt = new StreamReader(new FileStream("Assets/TextData/HappeningData.txt", FileMode.Open));
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

    // 메모장에서 값 읽어서 저장하는 함수
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
        happeningOccurCnt.Add(id, occurCnt); // 발생횟수

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

    // 진행상황 불러오기
    public bool LoadProgress()
    {

        return true;
    }
    // 이벤트 진행 순서 새로 만들기
    private void MakeProgress()
    {
        //happeningStream.Add(1);
        happeningStream = new List<Tuple<int, int>>();

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
        Debug.Log("이벤트간 평균 간격 : " + happeningBetweenTerm.ToString());

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
                        while(ID == beforeIdx){
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
    }

    // 다음 이벤트을 가져오는 함수
    public Tuple<int,int> GetNextHappening__(){
        if(happeningStream.Count <= presentHappeningIdx){
            return new Tuple<int,int>(-1,-1);
        }
        return happeningStream[presentHappeningIdx++];
    }
    // 이벤트 디버그 출력
    public void DebugPrintHappening__(Tuple<int,int> hpng){
        if (hpng.Item2 == -1)
        {
            Debug.Log("No Happenings");
        }
        else
        {
            Debug.Log(happeningTitles[hpng.Item2] + " " + hpng.Item1.ToString() + " " +
             IntToDateString(hpng.Item1) + " " + (dayOfTheWeek)DateType(hpng.Item1));
        }
    }

    // 방학인지 물어보는 함수
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
            if(happeningList[i] == beforeIdx){
                continue;
            }
            if(happeningOccurCnt[happeningList[i]] <= 0){
                continue;
            }
            bool flag = false;
            foreach(var rangeEach in happeningOccurRange[happeningList[i]]){
                if(rangeEach.Item1 <= dateNow && dateNow <= rangeEach.Item2){
                    flag = true;
                    break;
                }
            }
            if(flag){
                randomVal.Add(i);
            }
        }
        if(randomVal.Count == 0)return -1;


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


    // int값을 년/월/일 튜플로 반환
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
    // int값을 string으로 반환
    public string IntToDateString(int convertDate)
    {
        Tuple<int, int, int> date = IntToDate(convertDate);
        return date.Item1.ToString() + "/" + date.Item2.ToString() + "/" + date.Item3.ToString();
    }
    // 년/월/일 튜플을 int값으로 반환
    public int DateToInt(Tuple<int, int, int> convertDate)
    {
        int ret = 0;
        ret += convertDate.Item1 * 365;
        ret += convertDate.Item3;
        for (int i = 1; i < convertDate.Item2; i++) ret += Month[i];
        return ret;
    }
    // 년/월/일 문자열을 튜플로 반환
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
    // 년/월/일 문자열을 int값으로 반환
    public int StringToInt(String convertDate)
    {
        return DateToInt(StringToDate(convertDate));
    }
    // 요일 계산기
    public int DateType(int convertDate)
    {
        return ((convertDate - 1) % 7);
    }
    // 범위에서 랜덤 뽑기 (요일 범위 정하고, 거를 요일 비트연산으로 넣어서 전달)
    public int GetRandomInRange(List<Tuple<int, int>> rangeList, List<Tuple<int, int>> excludeRangeList = null, int excludeDateType = 0, int offset = 0)
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
}