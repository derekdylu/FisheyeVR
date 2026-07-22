using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSpawner : MonoBehaviour
{
    public GameObject[] target;
    int frame = 0;
    Vector3 center = new Vector3(43.187f, 0f, -80.547f);
    float radius = 5f;
    bool spawnNewOne = true;

    // Update is called once per frame
    void Update()
    {
        if (spawnNewOne) {
            float angle = Random.Range(0f, 2*Mathf.PI);
            Vector3 position = new Vector3(center.x+Mathf.Sin(angle)*radius, 0f, center.z-Mathf.Cos(angle)*radius);
            Instantiate(target[Random.Range(0, target.Length)], position, Quaternion.Euler(0f, 180f-angle*Mathf.Rad2Deg, 0f));
            spawnNewOne = false;
        }
    }

    public void CallSpawnNewOne() {
        spawnNewOne = true;
    }
}
