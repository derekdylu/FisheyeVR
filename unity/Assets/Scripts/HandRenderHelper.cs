using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class HandRenderHelper : MonoBehaviour
{
    public GameObject obj;
    public Collider objCollider;

    public GameObject obj2;
    public Collider objCollider2;

    public Camera cam;
    // Plane[] planes;

    void Start()
    {
        // cam = Camera.main;
        // planes = GeometryUtility.CalculateFrustumPlanes(cam);
        // objCollider =  GetComponent<Collider>();
    }

    void Update()
    {
        // VRUtils.Instance.Log("camera y: " + cam.transform.position.y);
        // VRUtils.Instance.Log("right hand y: " + obj.transform.position.y);
        // VRUtils.Instance.Log("left hand y: " + obj2.transform.position.y);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        // for (int i = 0; i < 6; ++i)
        // {
        //     GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //     p.name = "Plane " + i.ToString();
        //     p.transform.position = -planes[i].normal * planes[i].distance;
        //     p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[i].normal);
        // }

        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds) && GeometryUtility.TestPlanesAABB(planes, objCollider2.bounds))
        {
            // Debug.Log(obj.name + " has been detected!");
            FocalLengthSetting.instance.HandHelperFalse();
        }
        else
        {
            // Debug.Log("Not two objects have been detected");
            FocalLengthSetting.instance.HandHelperTrue();
        }
    }
}
