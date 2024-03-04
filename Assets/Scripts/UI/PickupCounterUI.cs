using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupCounterUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private PickupItemCounter _counter;

    private void Awake()
    {
        _counter.OnCounterChange += SetValue;
    }

    private void SetValue(int value)
    {
        _text.text = value.ToString();
    }
}
