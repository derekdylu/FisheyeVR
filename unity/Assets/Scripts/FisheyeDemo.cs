using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using BNG;

public class FisheyeDemo : MonoBehaviour
{
    public TMP_Text title;
    public GameObject others;
    public GameObject wholeLoading;

    int frame = 0;
    int inputFrame = 0;
    bool work = true;

    // Start is called before the first frame update
    void Start()
    {
        title.text = "Loading...";
        others.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (work) {
        inputFrame++;
            if (frame >= 200) {
                title.text = "FisheyeVR";
                others.SetActive(true);
            } else {
                frame++;
            }

            if ((InputBridge.Instance.AButton || InputBridge.Instance.BButton || InputBridge.Instance.XButton || InputBridge.Instance.YButton) && inputFrame >= 20) {
                inputFrame = 0;
                wholeLoading.SetActive(false);
                work = false;
            }
        }
    }
}
