using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : PickupItem
{
    [SerializeField]
    private GameObject _objToDestroy;
    public override void DestroyAfterPickup()
    {
        Destroy(_objToDestroy);
    }

    public override void OnPickup()
    {
    }
}
