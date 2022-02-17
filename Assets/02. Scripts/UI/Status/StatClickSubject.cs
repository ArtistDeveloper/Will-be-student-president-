using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UI.status
{
    public class StatClickSubject : MonoBehaviour, IObserverSubject
    {
        // status가 클릭되고 클릭이 취소될 것을 담은 delegate
        public List<StatClick> StatClickDelegateList { get; set; }
        public List<StatClickCancle> StatClickCancleDelegateList { get; set; }

        private const int StatusKindLength = 4;
        private StatClick click;
        private StatClickCancle clickCancle;


        private void Awake()
        {
            InitList();
        }

        public void RegisterObserver(StatusKinds statusKinds, StatEvent function, StatEvetType statEvetType) // 원래는 statclick
        {

            if (statEvetType == StatEvetType.Click)
            {
                click = new StatClick(function);

                // if (StatClickDelegateList[(int)statusKinds].GetInvocationList() != null)
                if (StatClickDelegateList[(int)statusKinds] != null)
                {
                    foreach (StatClick n in StatClickDelegateList[(int)statusKinds].GetInvocationList())
                    {
                        if (n == click)
                        {
                            Debug.Log("이미 해당 함수가 Delegate에 존재하고 있음");
                            return;
                        }
                    }
                }

                StatClickDelegateList[(int)statusKinds] += click;
            }

            else if (statEvetType == StatEvetType.ClickCancle)
            {
                clickCancle = new StatClickCancle(function);

                // if (StatClickDelegateList[(int)statusKinds].GetInvocationList() != null)
                if (StatClickCancleDelegateList[(int)statusKinds] != null)
                {
                    foreach (StatClick n in StatClickCancleDelegateList[(int)statusKinds].GetInvocationList())
                    {
                        if (n == click)
                        {
                            Debug.Log("이미 해당 함수가 Delegate에 존재하고 있음");
                            return;
                        }
                    }
                }

                StatClickCancleDelegateList[(int)statusKinds] += clickCancle;
            }
        }


        public void RemoveObserver()
        {

        }

        public void NotifyObservers()
        {

        }

        // List초기화 안되는 문제 때문에 제작한 함수
        private void InitList()
        {
            StatClickDelegateList = new List<StatClick>();
            StatClickCancleDelegateList = new List<StatClickCancle>();

            StatClick dummyStatClick = null;
            StatClickCancle dummystatClickCancle = null;

            for (int i = 0; i < StatusKindLength; i++)
            {
                StatClickDelegateList.Add(dummyStatClick); // dummy
                StatClickCancleDelegateList.Add(dummystatClickCancle);
            }
        }
    }
}

