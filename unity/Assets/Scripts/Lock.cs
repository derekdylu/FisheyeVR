using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lock : MonoBehaviour
{
    public GameObject Switch1;
    public GameObject Switch2;
    public GameObject Door1;
    public GameObject Door2;
    bool switch1 = false;
    bool switch2 = false;
    public TMP_Text text1;
    public TMP_Text text2;

    public AudioSource resetSound;
    bool played = false;
    // public GameObject Light1;
    // public GameObject Light2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (switch1 && switch2) {
            if (!played) {
                PlaySound();
                Notification.instance.Show("Doors Unlocked!");
            }
            Door1.SetActive(false);
            Door2.SetActive(false);
        }
    }

    public void Switch1Press () {
        switch1 = true;
        text1.text = "OFF";
        // Light1.SetActive(false);
    }

    public void Switch2Press () {
        switch2 = true;
        text2.text = "OFF";
        // Light2.SetActive(false);
    }

    void PlaySound () {
        resetSound.Play();
        played = true;
    }
}
