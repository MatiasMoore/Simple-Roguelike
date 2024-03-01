using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField]
    private Weapon _weapon;

    public void Init(InputSystem inputSystem)
    {
        SetActiveWeapon(_weapon);
        inputSystem.ReloadEvent += Reload;
    }

    public void SetActiveWeapon(Weapon weapon)
    {
        _weapon = weapon;
        _weapon.Init();
    }

    private void Update()
    {
        if (InputSystem.IsCursorPressed)
            Fire(InputSystem.CursorPosition);
        _weapon.RotateWeaponToPoint(InputSystem.CursorPosition);
    }

    private void Fire(Vector2 direction)
    {
        Debug.Log($"{transform.name} fired to {direction}!");
        _weapon.Enter();

    }

    private void Reload()
    {
        if (_weapon is Rifle)
        {
            (_weapon as Rifle).Reload();
        }        
    }

}
