using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

abstract public class PickupItemCounter : MonoBehaviour
{
    [SerializeField]
    private CollisionListener _listener;

    [SerializeField]
    private int _counter = 0;

    public UnityAction<int> OnCounterChange;

    private void Awake()
    {
        _listener.OnTriggerEnter += TryToPickup;
    }

    abstract public bool ShouldPickupItem(PickupItem item);

    protected void SetCounterValue(int newValue)
    {
        _counter = newValue;
        OnCounterChange?.Invoke(newValue);
    }

    private void TryToPickup(Collider2D collider)
    {
        var pickup = collider.GetComponent<PickupItem>();
        if (pickup != null && ShouldPickupItem(pickup))
        {
            SetCounterValue(_counter + pickup.GetItemCost());
            pickup.OnPickup();
            pickup.DestroyAfterPickup();
        }
    }
}
