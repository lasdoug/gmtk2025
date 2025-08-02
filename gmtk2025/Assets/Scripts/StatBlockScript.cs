using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StatBlockScript : MonoBehaviour
{
    public Texture2D up;
    public Texture2D down;
    public RawImage image;
    float lastRate = 100f;
    Tween pulseTween = null;
    Vector3 nativeScale;

    void Start()
    {
        image.texture = up;
        nativeScale = image.transform.localScale;
    }

    public void Rate(float rate)
    {
        if (rate <= 0)
        {
            image.texture = down;
        }
        else
        {
            image.texture = up;
        }

        rate *= 0.2f;
        if (Mathf.Abs(rate) < 0.045f)
        {
            if (pulseTween != null) pulseTween.Kill();
            image.transform.localScale = nativeScale;
            return;
        }
        if (!Mathf.Approximately(rate, lastRate))
        {
            if (pulseTween != null) pulseTween.Kill();
            lastRate = rate;
            pulseTween = image.transform.DOScale(rate + 1, 0.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }
}
