using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    public GameObject player;
    public GameObject door;
    bool status = false;
    public float distance = 2.0f;
    // true: opened, false: closed

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position - door.transform.position).magnitude < distance) {
            OpenDoor();
        } else {
            CloseDoor();
        }
    }

    void OpenDoor() {
        door.transform.localPosition = new Vector3(0.5f, 2.25f, 0f);
        status = true;
    }

    void CloseDoor() {
        door.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        status = false;
    }
}
