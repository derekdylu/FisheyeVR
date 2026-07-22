using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BreakableSpawnerPilot : MonoBehaviour
{
    public static BreakableSpawnerPilot instance;

    public GameObject target;

    Vector3 center = new Vector3(-34.88f, -0.93f, 85.09f);
    float radius = 5f;
    bool spawnNewOne = true;

    GameObject instantiateObject;
    Vector3 initialPosition;
    Vector3 position;
    Vector3 newPosition;
    float currentAngle;
    float currentHeight;
    float maxHeight = 4.879f;
    float minHeight = 0.109f;
    float horVelocityMax = 0.001f;
    int horDirection = 1;
    float verVelocityMax = 0.01f;
    int verDirection = 1;
    // public TMP_Text currentStage;

    private void Awake() {
        instance = this;
    }

    void Initialize() {
        horDirection = Random.Range(0, 2);
        if (horDirection == 0) {
            horDirection = -1;
        }
        float angle = Random.Range(0f, 2*Mathf.PI);
        currentAngle = angle;
        currentHeight = Random.Range(minHeight, maxHeight);
        initialPosition = new Vector3(center.x+Mathf.Sin(currentAngle)*radius, currentHeight, center.z+Mathf.Cos(currentAngle)*radius);
        instantiateObject = Instantiate(target, initialPosition, Quaternion.Euler(0f, 180f-currentAngle*Mathf.Rad2Deg, 0f));
        spawnNewOne = false;

        ProCamControl.instance.UpdateTarget();
    }

    void Satrt() {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnNewOne) {
            Initialize();
        }

        if (!spawnNewOne) {
            float newAngle = Random.Range(0, horDirection*horVelocityMax*Mathf.PI) + currentAngle;
            float newHeight = Random.Range(0, verDirection*verVelocityMax) + currentHeight;

            if (newHeight > maxHeight) {
                newHeight = maxHeight;
                verDirection = -1;
            }
            if (newHeight < minHeight) {
                newHeight = minHeight;
                verDirection = 1;
            }

            newPosition = new Vector3(center.x+Mathf.Sin(newAngle)*radius, newHeight, center.z+Mathf.Cos(newAngle)*radius);
            instantiateObject.transform.position = newPosition;

            currentAngle = newAngle;
            currentHeight = newHeight;
        }
    }

    public void CallSpawnNewOne() {
        spawnNewOne = true;
    }
}
