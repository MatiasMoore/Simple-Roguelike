using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> _weapons;

    private Weapon _currentWeapon;

    public void Init(InputSystem inputSystem)
    {
        if (_weapons.Count == 0)
        {
            Debug.LogError("No weapons found!");
            return;
        }
        SetActiveWeapon(_weapons[0]);

        inputSystem.ReloadEvent += Reload;
        inputSystem.ChooseNextWeapon += ChooseNextWeapon;
        inputSystem.ChoosePreviousWeapon += ChoosePreviousWeapon;
    }

    public void SetActiveWeapon(Weapon weapon)
    {
        _currentWeapon = weapon;
        _currentWeapon.Init();
    }

    private void Update()
    {
        if (InputSystem.IsCursorPressed)
            Fire(InputSystem.CursorPosition);
        _currentWeapon.RotateWeaponToPoint(InputSystem.CursorPosition);
    }

    private void Fire(Vector2 direction)
    {
        Debug.Log($"{transform.name} fired to {direction}!");
        _currentWeapon.Enter();

    }

    private void Reload()
    {
        if (_currentWeapon is Rifle)
        {
            (_currentWeapon as Rifle).Reload();
        }        
    }

    private void ChooseWeapon(int i)
    {
        // if this weapon already choosed
        if (_weapons[i] == _currentWeapon)
        {
            return;
        }
            
       _currentWeapon.Deinit();
        SetActiveWeapon(_weapons[i]);
    }

    private void ChooseNextWeapon()
    {
        int index = _weapons.IndexOf(_currentWeapon);
        index++;
        if (index >= _weapons.Count)
        {
            index = 0;
        }
        ChooseWeapon(index);
    }

    private void ChoosePreviousWeapon()
    {
        int index = _weapons.IndexOf(_currentWeapon);
        index--;
        if (index < 0)
        {
            index = _weapons.Count - 1;
        }
        ChooseWeapon(index);
    }
}
