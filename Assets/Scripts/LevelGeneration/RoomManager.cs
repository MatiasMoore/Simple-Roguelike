using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    [SerializeField]
    private GameObject doorRoot;

    [SerializeField]
    private GameObject floorRoot;

    public Vector3 GetDoorCoord(Door.DoorDirection direction)
    {
        var doors = doorRoot.GetComponentsInChildren<Door>();
        foreach (var door in doors)
        {
            if (door.GetDirection() == direction)
                return door.transform.position;
        }
        throw new System.Exception($"No door with direction({direction}) could be found for room({this.gameObject.name})");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
