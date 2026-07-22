using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterSystem : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    // Start is called before the first frame update
    void Start()
    {
        // left.SetActive(false);
        right.SetActive(false);
        // Debug.Log("right eye open at beginning");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (right.activeSelf == true){
            // left.SetActive(true);
            right.SetActive(false);
            // Debug.Log("switch to right eye open");
        } else {
            // left.SetActive(false);
            right.SetActive(true);
            // Debug.Log("switch to left eye open");
        }
    }
}
