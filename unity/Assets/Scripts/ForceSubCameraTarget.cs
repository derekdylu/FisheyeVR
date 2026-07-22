using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForceSubCameraTarget : MonoBehaviour
{
    public Camera leftCamera;
    public Camera rightCamera;

    public RenderTexture leftRenderTexture;
    public RenderTexture rightRenderTexture;

    // Start is called before the first frame update
    void Start()
    {
        leftCamera.targetTexture = leftRenderTexture;
        rightCamera.targetTexture = rightRenderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        leftCamera.targetTexture = leftRenderTexture;
        rightCamera.targetTexture = rightRenderTexture;
    }
}
