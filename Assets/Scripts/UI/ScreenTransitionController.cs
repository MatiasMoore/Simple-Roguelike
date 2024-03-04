using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransitionController : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    private int _matId = Shader.PropertyToID("_CircleSize");

    private const float _min = 0f;
    private const float _max = 1.5f;

    public void InstaFadeIn()
    {
        SetCircleSize(_max);
    }

    public void InstaFadeOut()
    {
        SetCircleSize(_min);
    }

    public void FadeIn(float time)
    {
        StartCoroutine(LerpCircleSize(_min, _max, time));
    }

    public void FadeOut(float time)
    {
        StartCoroutine(LerpCircleSize(_max, _min, time));
    }

    private IEnumerator LerpCircleSize(float from, float to, float time)
    {
        float totalTime = 0;
        var t = Mathf.Clamp01(totalTime / time);
        while (t < 1)
        {
            t = Mathf.Clamp01(totalTime / time);
            var current = Mathf.Lerp(from, to, t);
            SetCircleSize(current);
            totalTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void SetCircleSize(float size)
    {
        _image.materialForRendering.SetFloat(_matId, size);
        Debug.Log($"Shader set to {size}");
    }
}
