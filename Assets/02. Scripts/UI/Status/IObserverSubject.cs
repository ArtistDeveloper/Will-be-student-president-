using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UI.status
{
    public delegate void StatEvent();
    public delegate void StatClick();
    public delegate void StatClickCancle();

    public enum StatEvetType
    {
        Click,
        ClickCancle
    }

    public interface IObserverSubject
    {
        public void RegisterObserver(StatusKinds statusKinds, StatEvent function, StatEvetType statEvetType);
        public void RemoveObserver();
        public void NotifyObservers();
    }
}

