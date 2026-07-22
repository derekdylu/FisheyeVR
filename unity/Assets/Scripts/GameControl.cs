using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using BNG;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    int frame = 0;
    int fixedFrame = 0;
    bool pause = true;
    bool hasStarted = false;

    public GameObject menu;
    public GameObject bar;
    public GameObject test;
    public TMP_Text toggleBarVisibilityText;
    // public GameObject[] pointerCylinders;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text loggerIndicator;

    // Start is called before the first frame update

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        test.SetActive(false);
        menu.SetActive(false);
        bar.SetActive(false);
        // TogglePointerCylinders(false);
    }

    // Update is called once per frame
    void Update()
    {
        frame++;

        // show hide menu and pointer cylinders
        if (InputBridge.Instance.YButton && frame >= 20) {
            if (menu.activeSelf) {
                menu.SetActive(false);
                // TogglePointerCylinders(false);
                // LevelTeleport.instance.ShowMiniMap();
                ResumeTimer();
            } else {
                menu.SetActive(true);
                test.SetActive(false);
                // TogglePointerCylinders(true);
                // LevelTeleport.instance.HideMiniMap();
                PauseTimer();
            }
            frame = 0;
        }

        if (InputBridge.Instance.BButton && frame >= 20) {
            // if (pointerCylinders[0].activeSelf) {
            //     TogglePointerCylinders(false);
            // } else {
            //     TogglePointerCylinders(true);
            // }
            frame = 0;
        }

        // if (InputBridge.Instance.BButton && frame >= 20) {
        //     if (test.activeSelf) {
        //         ToggleTestVisibility(false);
        //     } else {
        //         ToggleTestVisibility(true);
        //     }
        //     frame = 0;
        // }

    }

    void FixedUpdate() {
        fixedFrame++;
        // if (fixedFrame/75 > 240) {
        //     timerText.text = "0";
        // }
        if (timerText.text == "0") {
            PauseTimer();
            UserTest.instance.Stop();
            ResetTimer();
            ToggleTestVisibility(true);
        }
        if (pause == false && Int32.Parse(timerText.text) > 0 && fixedFrame > 75) {
            timerText.text = (Int32.Parse(timerText.text) - 1).ToString();
            fixedFrame = 0;
        }
    }

    public void ToggleTestVisibility(bool status) {
        test.SetActive(status);
        menu.SetActive(false);
        if (status) {
            // TogglePointerCylinders(true);
            // LevelTeleport.instance.HideMiniMap();
            PauseTimer();
        } else {
            // TogglePointerCylinders(false);
            // LevelTeleport.instance.ShowMiniMap();
            ResumeTimer();
        }
    }

    public void ToggleBarVisibility() {
        if (bar.activeSelf) {
            bar.SetActive(false);
            toggleBarVisibilityText.text = "Show Bar";
        } else {
            bar.SetActive(true);
            toggleBarVisibilityText.text = "Hide Bar";
        }
    }

    // public void TogglePointerCylinders(bool active) {
    //     foreach (GameObject cylinder in pointerCylinders) {
    //         cylinder.SetActive(active);
    //     }
    // }

    public void ResetScore() {
        scoreText.text = "0 Pt";
    }

    public void AddScore(int score) {
        LogWriter.instance.Tag("scored");
        int currentScore = Int32.Parse(scoreText.text.Split(' ')[0]);
        scoreText.text = (currentScore + score).ToString() + " Pt";
    }

    public void LessScore(int score) {
        int currentScore = Int32.Parse(scoreText.text.Split(' ')[0]);
        scoreText.text = (currentScore - score).ToString() + " Pt";
    }

    public void StartLogger() {
        loggerIndicator.text = "• REC";
    }

    public void StopLogger() {
        loggerIndicator.text = "";
    }

    public void StartTimer() {
        hasStarted = true;
        pause = false;
    }

    public void ResumeTimer() {
        if (hasStarted) {
            pause = false;
        }
    }

    public void PauseTimer() {
        pause = true;
    }

    public void ResetTimer() {
        timerText.text = "240";
        pause = true;
    }
}
