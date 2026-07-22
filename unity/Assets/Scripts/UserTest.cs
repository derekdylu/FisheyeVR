using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BNG;

public class UserTest : MonoBehaviour
{
    public static UserTest instance;

    public TMP_Text startText;
    bool playing = false;
    // public GameObject optionModule;
    public GameObject test;
    public AudioSource BGM;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!playing) {
            GameControl.instance.PauseTimer();
        }
    }

    public void Play() {
        playing = true;
        startText.text = "Leave";
        test.SetActive(false);
        // GameControl.instance.TogglePointerCylinders(false);
        GameControl.instance.ResetScore();
        GameControl.instance.ResetTimer();
        GameControl.instance.StartTimer();
        // optionModule.SetActive(false);
        LogWriter.instance.StartLogger();
        // SnapZoneTable.instance.ReleaseAll();
        BGM.Play();
        Enemy.instance.TurnOn();
    }

    public void Stop() {
        LogWriter.instance.StopLogger();
        test.SetActive(true);
        playing = false;
        startText.text = "Start!";
        // GameControl.instance.TogglePointerCylinders(true);
        GameControl.instance.PauseTimer();
        // optionModule.SetActive(true);
        BGM.Pause();
        Enemy.instance.TurnOff();
    }

    public void ClickPlay() {
        if (playing) {
            Stop();
        } else {
            Play();
        }
    }

}
