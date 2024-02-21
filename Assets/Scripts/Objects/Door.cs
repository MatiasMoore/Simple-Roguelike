using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorDirection
    {
        up, down, left, right
    }

    public DoorDirection GetDirection()
    {
        var doorRot = this.transform.rotation.z;

        switch (doorRot)
        {
            case 0:
                return DoorDirection.up;
            case 90:
                return DoorDirection.left;
            case -90:
                return DoorDirection.right;
            case 180:
                return DoorDirection.down;
            default:
                throw new System.Exception($"Unable to get door({this.gameObject.name}) direction");
        }
    }
}
