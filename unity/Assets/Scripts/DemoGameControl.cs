using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using BNG;

public class DemoGameControl : MonoBehaviour
{

    public static DemoGameControl instance;

    int frame = 0;

    public GameObject menu;
    public GameObject[] pointerCylinders;
    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        TogglePointerCylinders(false);
    }

    // Update is called once per frame
    void Update()
    {
        frame++;

        // show hide menu and pointer cylinders
        if (InputBridge.Instance.YButton && frame >= 20) {
            if (menu.activeSelf) {
                menu.SetActive(false);
                TogglePointerCylinders(false);
                // LevelTeleport.instance.ShowMiniMap();
                // ResumeTimer();
            } else {
                menu.SetActive(true);
                // test.SetActive(false);
                TogglePointerCylinders(true);
                // LevelTeleport.instance.HideMiniMap();
                // PauseTimer();
            }
            frame = 0;
        }
    }

    public void TogglePointerCylinders(bool active) {
        foreach (GameObject cylinder in pointerCylinders) {
            cylinder.SetActive(active);
        }
    }
}
