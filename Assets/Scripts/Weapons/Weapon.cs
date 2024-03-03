using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] 
    public WeaponDataSO WeaponData { get; private set; }

    public abstract void Init();

    public abstract void Enter();

    public abstract void Deinit();

    public abstract void RotateWeaponToPoint(Vector2 position);


}
