using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class CollisionListener : MonoBehaviour
{
    public UnityAction<Collider2D> OnTriggerEnter;

    public UnityAction<Collider2D> OnTriggerExit;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit?.Invoke(collision);
    }

    public void SetRadius(float radius)
    {
        GetComponent<CircleCollider2D>().radius = radius;
    }

    public float GetRadius()
    {
        return GetComponent<CircleCollider2D>().radius;
    }

    public void SetColliderActive(bool active)
    {
        GetComponent<CircleCollider2D>().enabled = active;
    }
}
