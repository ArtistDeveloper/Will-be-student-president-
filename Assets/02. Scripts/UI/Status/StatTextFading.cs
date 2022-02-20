using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.status
{
    public class StatTextFading : MonoBehaviour
    {
        private Text statText;
        private StatusKinds statusKind;
        private StatClickSubject statClickSubject;

        private void Start()
        {
            statText = GetComponent<Text>();
            Color initAlpha = statText.color;
            initAlpha.a = 0f;
            statText.color = initAlpha; 

            Transform rootParent = transform.parent.parent;
            statClickSubject = rootParent.GetComponent<StatClickSubject>();

            statusKind = Stat.DistinguishStatkinds(transform.parent.name);
            statClickSubject.RegisterObserver(statusKind, ShowText, StatEvetType.Click);
            statClickSubject.RegisterObserver(statusKind, HideText, StatEvetType.ClickCancle);
        }

        public void HideText()
        {
            statText.DOFade(0f, 1f).SetAutoKill<Tween>();
        }

        public void ShowText()
        {
            statText.DOFade(1f, 1f).SetAutoKill<Tween>();
        }
    }
}

