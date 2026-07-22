using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using BNG;

public class FocalLengthSetting : MonoBehaviour
{
    public static FocalLengthSetting instance;

    private void Awake() {
        instance = this;
    }

    public TMP_Text fovText;
    public TMP_Text modeText;
    public TMP_Text wideText;
    public TMP_Text wideFovText;
    public TMP_Text sensitivityText;

    public Camera camera; // for manipulation lens
    public Camera mainCamera; // for main camera (intact)

    public GameObject renderObject;
    public GameObject rightHandAnchor;
    public GameObject leftHandAnchor;

    public GameObject postProcessObject;

    int frame = 0;

    float currentFieldOfView;
    float defaultFieldOfView = 94.14985f;
    float wideFieldOfView = 124.0f;

    float[] wideList = {104.0f, 114.0f, 124.0f, 134.0f, 144.0f};
    string[] wideTextList = {"less wide", "normal wide", "more wide", "super wide", "extremely wide"};
    int wideIndex = 1;

    float headFixationAngle = 5f;
    int headFixationDuration = 2;

    float[] angleList = {0.5f, 1f, 2.5f, 5f, 7.5f};
    string[] angleTextList = {"extremely sensitive", "super sensitive", "more sensitive", "normal sensitive", "less sensitive"};
    int angleIndex = 3;

    float currentAngle = 0;
    List<float> angles = new List<float>();
    float previousAngle;
    bool headMaintain = false;
    int headFrame = 0;

    Vector3 currentMainCameraPosition;
    public Vector3 dollyOutCameraPosition = new Vector3(0,0,-0.79208f);
    Vector3 defaultCameraPosition = new Vector3(0,0,0);

    bool dolly = false;
    bool trigger = false;
    bool hybridByHand = false;

    int mode = 1;
                    // 0: none
                    // 1: zoom manual 2: zoom head rotation 3: zoom hand position 4: zoom hybrid
    bool handMode = false;
    string[] modeNames = {"none", "zoom manual", "zoom head", "zoom hand", "zoom hybrid"};

    void Start()
    {
        wideFieldOfView = wideList[wideIndex];
        wideText.text = wideTextList[wideIndex];
        headFixationAngle = angleList[angleIndex];
        sensitivityText.text = angleTextList[angleIndex];

        currentFieldOfView = defaultFieldOfView; // Quest 2 default is 94.14985 vertically
        camera.fieldOfView = currentFieldOfView;
        currentMainCameraPosition = mainCamera.transform.localPosition;
        fovText.text = "--default--";
        modeText.text = modeNames[mode];
        trigger = false;

        ModeCheck();
    }

    void Update()
    {
        frame++;

        camera.fieldOfView = currentFieldOfView;
        if (!handMode)   trigger = false;

        // change trigger mode
        if (InputBridge.Instance.BButton && frame >= 20) {
            ModeSwitch();
            frame = 0;
        }

        // change wide field of view value
        // if (InputBridge.Instance.AButton && frame >= 20) {
        //     wideIndex = (wideIndex + 1) % 5;
        //     wideText.text = wideTextList[wideIndex];
        //     wideFieldOfView = wideList[wideIndex];
        //     frame = 0;
        // }

        // change sensitivity
        // if (InputBridge.Instance.XButton && frame >= 20) {
        //     angleIndex = (angleIndex + 1) % 4;
        //     sensitivityText.text = angleTextList[angleIndex];
        //     headFixationAngle = angleList[angleIndex];
        //     frame = 0;
        // }

        // reset all
        if (InputBridge.Instance.YButton && frame >= 20) {
            ShuffleClusters.instance.Reset();
            frame = 0;
        }

        // toggle postprocessing
        if (InputBridge.Instance.XButton && frame >= 20) {
            if (postProcessObject.activeSelf) {
                postProcessObject.SetActive(false);
            } else {
                postProcessObject.SetActive(true);
            }
        }

        ModeCheck();

        if (mode == 0) {
            return;
        }
        if (mode == 1) {
            Manual();
        }
        if (mode == 2) {
            Head();
        }
        if (mode == 3) {
            // hand
        }
        if (mode == 4) {
            // hand and
            Head();
        }

        if (trigger) {
            fovText.text = "--wide--";
            if (!dolly) {
                currentFieldOfView = Mathf.Lerp(currentFieldOfView, wideFieldOfView, 0.1f);
            } else {
                currentMainCameraPosition = Vector3.Lerp(currentMainCameraPosition, dollyOutCameraPosition, 0.15f);
                mainCamera.transform.localPosition = currentMainCameraPosition;
            }
        } else {
            fovText.text = "--default--";
            if (!dolly) {
                currentFieldOfView = Mathf.Lerp(currentFieldOfView, defaultFieldOfView, 0.1f);
            } else {
                currentMainCameraPosition = Vector3.Lerp(currentMainCameraPosition, defaultCameraPosition, 0.15f);
                mainCamera.transform.localPosition = currentMainCameraPosition;
            }
        }
        wideFovText.text = wideFieldOfView.ToString();
    }

    void ModeCheck() {
        // toggle zoom renderer object
        if (mode == 0){
            renderObject.SetActive(false);
            mainCamera.farClipPlane = 1000f;
            mainCamera.nearClipPlane = 0.01f;
        } else {
            renderObject.SetActive(true);
            mainCamera.farClipPlane = 0.3f;
            mainCamera.nearClipPlane = 0.2f;
        }

        // toggle hand trigger
        if (mode == 3) {
            handMode = true;
        } else {
            handMode = false;
        }
    }

    void ModeSwitch() {
        mode = ( mode + 1 ) % 5;
        modeText.text = modeNames[mode];
        ModeCheck();
    }

    void Manual() {
        if (InputBridge.Instance.RightThumbstickAxis != Vector2.zero) {
            trigger ^= true;
        }
    }

    void Head() {
        headFrame++;

        previousAngle = currentAngle;
        currentAngle = mainCamera.transform.rotation.eulerAngles.y;

        float distance = Mathf.Abs(currentAngle - previousAngle);
        if (distance >= 180f) {
            if (previousAngle > currentAngle) {
                distance = (360f - previousAngle) + currentAngle;
            } else {
                distance = (360f - currentAngle) + previousAngle;
            }
        }

        if (distance > angleList[angleIndex]) {
            headMaintain = true;
            headFrame = 0;
        }

        if (headFrame > headFixationDuration * 60) {
            headMaintain = false;
        }

        if (headMaintain) {
            trigger = true;
            Debug.LogWarning("trigger by head");
        } else if (hybridByHand == false) {
            trigger = false;
        }
    }

    public void HandHelperTrue() {
        if (handMode) {
            trigger = true;
            Debug.LogWarning("trigger by hand");
            if (mode == 4 || mode == 8) {
                hybridByHand = true;
            }
        }
    }

    public void HandHelperFalse() {
        if (handMode) {
            trigger = false;
        }
    }

    float CorrectDegQuad(float x, float y, float d) {
        if (y >= 0f) {
            if (x > 0f) {
                return d;
            } else {
                return 180f - d;
            }
        }
        if (y < 0f) {
            if (x > 0f) {
                return 360f - d;
            } else {
                return 180f + d;
            }
        }
        return 0f;
    }
}
