using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeDetection : MonoBehaviour
{
    public GameObject player;

    public GameObject[] detections;

    public TMP_Text stage;

    float radius = 6f;
    int currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (stage.text == "3 maze") {
            for (int i = 0; i < detections.Length; i++) {
                if (Vector3.Distance(player.transform.position, detections[i].transform.position) < radius) {
                    if ( i != currentLevel) {
                        int lvl = i + 1;
                        string lvl_s = lvl.ToString();
                        print("entering level" + lvl_s);
                        LogWriter.instance.Tag("entering level" + lvl_s);
                        currentLevel = i;
                    }
                }
            }
        }
    }
}