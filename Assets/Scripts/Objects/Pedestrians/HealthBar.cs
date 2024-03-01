using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth = 1;

    [SerializeField]
    private float _currentHealth = 1;

    [SerializeField]
    private Slider _slider;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = currentHealth;
        UpdateSlider();
    }

    public void SetCurrenthealth(float currentHealth)
    {
        _currentHealth = currentHealth;
        UpdateSlider();
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        if (_maxHealth == 0)
        {
            Debug.LogWarning($"Max health is 0 for {gameObject.name}");
        }
        _slider.value = _currentHealth / _maxHealth;
    }
}
