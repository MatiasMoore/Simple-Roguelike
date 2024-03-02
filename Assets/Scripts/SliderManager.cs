using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField]
    private float _maxValue = 1;

    [SerializeField]
    private float _currenValue = 1;

    [SerializeField]
    private Slider _slider;

    public void UpdateValues(float currenValue, float maxValue)
    {
        _maxValue = maxValue;
        _currenValue = currenValue;
        UpdateSlider();
    }

    public void SetCurrentValue(float currenValue)
    {
        _currenValue = currenValue;
        UpdateSlider();
    }

    public void SetMaxValue(float maxValue)
    {
        _maxValue = maxValue;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        if (_maxValue == 0)
        {
            Debug.LogWarning($"Max health is 0 for {gameObject.name}");
        }
        _slider.value = _currenValue / _maxValue;
    }
}
