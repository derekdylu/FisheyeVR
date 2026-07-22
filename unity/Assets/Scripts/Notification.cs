using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    // create instance
    public static Notification instance;

    // awake it
    private void Awake() {
        instance = this;
    }

    public GameObject Canvas;
    public TMP_Text text;
    public AudioSource notificationSound;
    int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        if (frame >= 250) {
            Canvas.SetActive(false);
        }
    }

    public void ChangeText (string str) {
        text.text = str;
    }

    public void Show (string str) {
        notificationSound.Play();
        text.text = str;
        Canvas.SetActive(true);
        frame = 0;
    }
}
