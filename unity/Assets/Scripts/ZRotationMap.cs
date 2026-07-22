using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZRotationMap : MonoBehaviour
{
    public GameObject player;
    public GameObject alternativeSystem;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        alternativeSystem.transform.localRotation = Quaternion.Euler(0, 0, player.transform.rotation.eulerAngles.z);
    }
}
