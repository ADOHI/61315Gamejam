using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Adohi.Managers.UIs
{
    public class GlobalUIManager : Singleton<GlobalUIManager>
    {
        public Image fadeImage;



        public void Fade(float duration, float alpha)
        {
            fadeImage.DOFade(alpha, duration).SetUpdate(true);
        }

        public async UniTask FadeAsync(float duration, float alpha)
        {
            await fadeImage.DOFade(alpha, duration).SetUpdate(true);
        }
    }

}
