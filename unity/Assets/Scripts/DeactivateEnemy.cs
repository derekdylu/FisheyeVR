using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateEnemy : MonoBehaviour
{
    public GameObject enemy;
    public bool deactivate = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (deactivate) {
            deactivate = false;
            enemy.SetActive(false);
            ProCamControl.instance.SetAlertMode(false);
        }
    }
}
