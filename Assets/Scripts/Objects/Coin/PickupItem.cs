using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
abstract public class PickupItem : MonoBehaviour
{
    [SerializeField]
    protected int _cost = 1;
    public int GetItemCost()
    {
        return _cost;
    }

    abstract public void OnPickup();

    abstract public void DestroyAfterPickup();
}
