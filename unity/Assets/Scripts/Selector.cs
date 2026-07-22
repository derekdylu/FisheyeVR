using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class Selector : MonoBehaviour
{
    private LineRenderer lineRenderer;
    GameObject selectedObject;
    int frame = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        frame++;
        RaycastHit hit;
        // Create a ray that goes forward from the pointer
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit))
        {
            // The ray has hit something
            Debug.Log("Pointing at: " + hit.collider.gameObject.name);

            // Access the hit object's information
            // For example, accessing its Transform
            Transform hitObjectTransform = hit.transform;

            // You can also access other components like this
            // MyCustomScript customScript = hit.collider.GetComponent<MyCustomScript>();

            // Do something with the hit object information
            if (InputBridge.Instance.AButton && frame >= 20) {
                Debug.Log("Set selected object: " + hit.collider.gameObject.name);
                selectedObject = hit.collider.gameObject;
                ProCamControl.instance.UpdateTargetTag(0, selectedObject.tag);
                frame = 0;
            }
        }
    }
}

