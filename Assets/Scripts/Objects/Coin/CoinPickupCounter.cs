using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickupCounter : PickupItemCounter
{
    public override bool ShouldPickupItem(PickupItem item)
    {
        var coin = (CoinPickup)item;
        return coin != null;
    }
}
