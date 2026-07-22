using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetection : MonoBehaviour
{
    public bool isRunning = true;
    public GameObject player;
    public GameObject[] levelAnchors;
    int current = -1;
    float radius = 6.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning) {
            DetectLevel();
        }
    }

    void DetectLevel() {
        for (int i = 0; i < levelAnchors.Length; i++) {
            if (Vector3.Distance(player.transform.position, levelAnchors[i].transform.position) < radius) {
                if (current != i) {
                    current = i;
                    LogWriter.instance.Tag("maze_" + i.ToString());
                }
                break;
            }
        }
    }
}
