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
        InputSystem.Instance.CursorClickEvent += Fire;
        SetActiveWeapon(_weapon);
    }

    public void SetActiveWeapon(Weapon weapon)
    {
        _weapon = weapon;
        _weapon.Init();
    }

    private void Update()
    {
        _weapon.RotateWeaponToPoint(InputSystem.CursorPosition);
    }

    private void Fire(Vector2 direction)
    {
        Debug.Log($"{transform.name} fired to {direction}!");
        _weapon.Enter();

    }

}
