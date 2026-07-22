using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System;
using BNG;

public class FisheyeVR : MonoBehaviour
{
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // text lens switch every 1 second
        if (Time.time % 1 < Time.deltaTime) {
           CameraBody.instance.LensSwitch(i);
           i++;
        }
    }
}
