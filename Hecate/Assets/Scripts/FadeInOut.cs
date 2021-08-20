using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] private Image _panel;
    [SerializeField] private Volume _volume;

    private Sequence _fadeInOutTween = null;

    private void Awake()
    {
        _fadeInOutTween = DOTween.Sequence();

        _fadeInOutTween.Append(DOTween.To(() => _volume.weight, weight => _volume.weight = weight, 1, 0.3f).From(0)
            .SetEase(Ease.InQuad));
        _fadeInOutTween.Append(_panel.DOFade(1, 0.05f).SetEase(Ease.Linear).From(0));

        _fadeInOutTween.SetAutoKill(false);

        _fadeInOutTween.Rewind();
    }


    [Button]
    public void FadeIn()
    {
        _fadeInOutTween.Restart();        
    }
    
    [Button]
    public void FadeOut()
    {
        _fadeInOutTween.SmoothRewind();
    }
}
