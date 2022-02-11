using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;


namespace UI.status
{
    public class StatusClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEvent statClickEvent;
        [SerializeField] private UnityEvent statClickCancleEvent;

        public Image testImage;

        private Tween fadeOutTween;
        private Tween fadeInTween;

        private bool isPressed;


        private void Start()
        {
            StartCoroutine(CoCheckClickEvent());
        }

        private IEnumerator CoCheckClickEvent()
        {
            while (true)
            {
                yield return null; // while에서 빠져나올 수 있도록 작업제어권 반환 // 여기 선언안하면 continue 때문에 터짐

                if (isPressed)
                {
                    // 버튼이 눌러지고 있고, fadeInTween이 실행되고 있다면, fadeInTween 종료.
                    if (fadeInTween != null)
                    {
                        fadeInTween.Kill(false);
                        fadeInTween = null;
                    }

                    // 버튼이 눌러지고 있는데, fadeOutTween이 이미 실행중이라면 추가 실행X. //근데 이게 작동을 안하네;?
                    if (fadeOutTween != null)
                    {
                        continue;
                    }

                    FadeOut();
                }
                else
                {
                    // 버튼이 눌러지지 않고, fadeInTween이 실행중이라면 추가 실행X.
                    if (fadeInTween != null)
                    {
                        continue;
                    }

                    // 버튼이 눌러지지 않고, fadeOutTween이 종료되지 않았다면, fadeOutTween 종료 후 FadeIn실행.
                    if (fadeOutTween != null)
                    {
                        fadeOutTween.Kill(false);
                        fadeOutTween = null;

                        Debug.Log("Fade In 호출중");
                        FadeIn();
                    }
                }
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            statClickEvent.Invoke();
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            statClickCancleEvent.Invoke();
        }


        public void FadeIn()
        {
            fadeInTween = testImage.DOFade(1, 3);
            fadeInTween.onComplete += () =>
            {
                fadeInTween.Kill(false);
                fadeInTween = null;
            };
        }


        public void FadeOut()
        {
            //Fade Out은 끝나도, 마우스 클릭이 계속 되고있으면 FadeIn이 실행되면 안되기에 OnComplete에 다른 것을 붙여주지 않는다.
            Debug.Log("호출됐는데 왜그랭");
            fadeOutTween = testImage.DOFade(0, 3);
        }


        private void Fade(float endValue, float duration)
        {
            // fadeTween = testImage.DOFade(endValue, duration);
        }
    }
}

