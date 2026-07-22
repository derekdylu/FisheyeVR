using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedFieldOfView : MonoBehaviour
{
    public Camera mainCamera;
    public float forcedFieldOfView = 134.0f;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera.fieldOfView = forcedFieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.fieldOfView = forcedFieldOfView;
    }
}
