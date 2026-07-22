using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Laptops : MonoBehaviour
{
    public TMP_Text text1;
    public TMP_Text text2;
    public GameObject laptop1;
    public GameObject laptop2;
    public GameObject phone;
    float process1 = 0f;
    float process2 = 0f;
    public GameObject player;
    bool hacked = false;
    bool decrypted = false;

    public AudioSource hackingSound;
    bool playedHacking = false;
    bool playedHacking2 = false;
    public AudioSource decryptedSound;
    bool playedDecrypted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!decrypted) {
            text1.text = "Hacking: " + process1.ToString("F2");
            text2.text = "Decrypting: " + process2.ToString("F2");
        } else {
            text1.text = "Hacked";
            text2.text = "Decrypted";
        }

        if (Vector3.Distance(player.transform.position, laptop1.transform.position) < 1f) {
            if (!playedHacking) {
                PlaySound(1);
            }
            Laptop1Process();
        } else {
            // if (!playedHacking && !playedHacking2) {
            //     hackingSound.Pause();
            // }
        }

        if (Vector3.Distance(player.transform.position, laptop2.transform.position) < 1f && hacked) {
            if (!playedHacking2) {
                PlaySound(2);
            }
            Laptop2Process();
        } else {
            // if (playedHacking) {
            //     hackingSound.Pause();
            // }
        }
    }

    public void Laptop1Process () {
        if (process1 >= 100f) {
            process1 = 100f;
            hacked = true;
            hackingSound.Stop();
            Notification.instance.Show("Password Hacked!");
            return;
        } else {
            process1 += 0.2f;
        }
    }

    public void Laptop2Process () {
        if (decrypted) {
            return;
        }
        if (process2 >= 100f) {
            process2 = 100f;
            decrypted = true;
            if (!playedDecrypted) {
                hackingSound.Stop();
                PlaySound2();
            }
            Notification.instance.Show("Data Stolen!");
            GameControl.instance.AddScore(10);
            return;
        } else {
            process2 += 0.2f;
        }
    }

    void PlaySound (int count) {
        hackingSound.Play();
        if (count == 1){
            playedHacking = true;
        } else if (count == 2) {
            playedHacking2 = true;
        }
    }

    void PlaySound2 () {
        decryptedSound.Play();
        playedDecrypted = true;
    }
}
