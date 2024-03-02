using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    [SerializeField]
    private Text _textField;

    [SerializeField]
    private SliderManager _reloadProgressBar;

    [SerializeField]
    private int _maxAmmo;

    [SerializeField]
    private int _currentAmmo;

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        _currentAmmo = currentAmmo;
        _maxAmmo = maxAmmo;
        UpdateAmmoText();
    }

    public void SetCurrentAmmo(int currentAmmo)
    {
        _currentAmmo = currentAmmo;
        UpdateAmmoText();
    }

    public void SetMaxAmmo(int maxAmmo)
    {
        _maxAmmo = maxAmmo;
        UpdateAmmoText();
    }

    public void UpdateAmmoText()
    {
        string ammoText = _currentAmmo + " / " + _maxAmmo;
        _textField.text = ammoText;
    }

    public void UpdateReloadTime(float currentReloadTime, float maxReloadTime)
    {
        _reloadProgressBar.UpdateValues(currentReloadTime, maxReloadTime);
    }

    public void SetActiveReloadBar(bool isActive)
    {
        _reloadProgressBar.gameObject.SetActive(isActive);
    }
}
