using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


namespace UI.status
{
    public class FadingUtil : MonoBehaviour
    {
        // public static Tween FadeOut(Image target)
        // {
        //     return target.DOFade(0, 1);
        // }

        // public static Tween FadeIn(Image target)
        // {
        //     // Tween fadeInTween = target.DOFade(0.5f, 1);
        //     // fadeInTween.onComplete += () =>
        //     // {
        //     //     fadeInTween.Kill(false);
        //     //     fadeInTween = null;
        //     // };

        //     Tween fadeInTween = target.DOFade(0.5f, 1).SetAutoKill<Tween>();
        //     return fadeInTween;
        // }

        public static Tween Fade(float endValue, float duration, Image target)
        {
            Tween fadeTween = target.DOFade(endValue, duration).SetAutoKill<Tween>();
            return fadeTween;
        }
    }
}

