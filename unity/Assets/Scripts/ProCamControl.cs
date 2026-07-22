using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System;
using BNG;

public class ProCamControl : MonoBehaviour
{
    public static ProCamControl instance;
    private void Awake() {
        instance = this;
    }

    public UnityEvent OnWide;
    public UnityEvent OnNarrow;

    public TMP_Text interfaceText;
    public TMP_Text methodText;
    public TMP_Text sensitivityText;
    public int stageIndex = 0;

    public GameObject mainCamera;

    bool triggerHead = false;
    int method = 5;

    int manualFrame = 0;
    bool manualLock = false;

    int headFrame = 0;
    float[] angleList = {0.5f, 2.5f, 5f};
    int sensIndex = 1;
    int headFixationDuration = 3;
    float currentAngle = 0;
    float previousAngle;
    bool headMaintain = false;

    public GameObject rightHandAnchor;
    public GameObject leftHandAnchor;

    float[] IdleFixationDurationList = {0.125f, 0.25f, 0.5f};
    int idleFrame = 0;

    public GameObject[] targets = {
        null, null, null, null
    };
    public string[] targetNames = {
        "", "Breakable(Clone)/GFX", "", "Crate(New)/CrateUndestroyed"
    };
    public string[] targetTags = {
        "", "Breakable", "", "Crates"
    };
    public GameObject frontAnchor;
    public float targetRadius = 3f;
    public TMP_Text radiusText;

    ////// linear system
    public int linearSystemLength = 21;
    public int linearSystemIndex = 0;
    float[] IdleHeadSensList = {0.333f, 0.667f, 1f};

    bool startToNarrow = false;
    bool alertMode = false;
    public bool officeGame = false;

    int interfaceModeIdx = 1;
    public bool log = false;

    // Start is called before the first frame update
    void Start()
    {
        InterfaceModeCheck(1);
        ResetTargetRadius();
        MethodModeCheck(5);
        SensitivityModeCheck(1);
        UpdateTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (interfaceModeIdx == 1) {

            if (method == 0) {
                Manual();
                return;
            }

            if (method == 1) {
                Head();
                return;
            }

            if (method == 2) {
                Hand();
                return;
            }

            if (method == 3) {
                Moving();
                return;
            }

            if (method == 4) {
                Target();
                return;
            }

            if (method == 5) {
                Holistic();
                return;
            }

        }
    }

    public void InterfaceModeCheck (int interfaceMode) {
        if (interfaceMode == 0) {
            interfaceText.text = "Static";
            interfaceModeIdx = 0;
        } else if (interfaceMode == 1) {
            interfaceText.text = "FisheyeVR";
            interfaceModeIdx = 1;
        }
    }

    public void MethodModeCheck (int methodMode) {
        if (methodMode == 0) {
            methodText.text = "Manual Switching";
            interfaceText.text = "FisheyeVR*";
            method = 0;
        } else if (methodMode == 1) {
            methodText.text = "Head Rotation";
            interfaceText.text = "FisheyeVR*";
            method = 1;
        } else if (methodMode == 2) {
            methodText.text = "Arm-Spreading";
            interfaceText.text = "FisheyeVR*";
            method = 2;
        } else if (methodMode == 3) {
            methodText.text = "Moving";
            interfaceText.text = "FisheyeVR*";
            method = 3;
        } else if (methodMode == 4) {
            methodText.text = "Target Incoming";
            interfaceText.text = "FisheyeVR*";
            method = 4;
        } else if (methodMode == 5) {
            methodText.text = "Holistic";
            interfaceText.text = "Full FisheyeVR";
            method = 5;
        }
    }

    public void handleMethodChange (int newMethod) {
        MethodModeCheck(newMethod);
    }

    public void SensitivityModeCheck (int sensitivityMode) {
        if (sensitivityMode == 0) {
            sensitivityText.text = "Less";
            sensIndex=2;
        } else if (sensitivityMode == 1) {
            sensitivityText.text = "Default";
            sensIndex=1;
        } else if (sensitivityMode == 2) {
            sensitivityText.text = "More";
            sensIndex=0;
        }
    }

    void Manual() {
        manualFrame++;

        if (InputBridge.Instance.BButton || InputBridge.Instance.RightThumbstickAxis != Vector2.zero) {
            LinearWider("Ma");
            // idleBufferFrame = 0;
        } else {
            LinearNarrower("Ma");
            // idleBufferFrame = 0;
        }

        LinearCheck();
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

        if (distance > angleList[sensIndex]) {
            headMaintain = true;
            headFrame = 0;
        }

        if (headFrame > headFixationDuration * 72) {
            headMaintain = false;
        }

        if (headMaintain) {
            triggerHead = true;
        } else {
            triggerHead = false;
        }
    }

    void Hand() {

        var a = frontAnchor.transform.position - mainCamera.transform.position;
        var b = rightHandAnchor.transform.position - mainCamera.transform.position;
        var c = leftHandAnchor.transform.position - mainCamera.transform.position;

        var aa = new Vector2(a.x, a.z);
        var bb = new Vector2(b.x, b.z);
        var cc = new Vector2(c.x, c.z);

        float angleRight = Vector2.Angle(aa, bb);
        float angleLeft = Vector2.Angle(aa, cc);

        // calculate hands distance
        float handsDistance = Vector3.Distance(rightHandAnchor.transform.position, leftHandAnchor.transform.position);

        if ( handsDistance < 1f || angleRight < (linearSystemIndex + 55) || angleLeft < (linearSystemIndex + 55)) {
            // print("hands narrow");
            LinearNarrower("Ha");
        } else {
            // print("hands wider");
            LinearWider("Ha");
        }

        LinearCheck();
    }

    void Target() {
        UpdateTarget();

        if (alertMode) {
            var a = frontAnchor.transform.position - mainCamera.transform.position;
            var b = targets[stageIndex].transform.position - mainCamera.transform.position;

            var aa = new Vector2(a.x, a.z);
            var bb = new Vector2(b.x, b.z);

            float angle = Vector2.Angle(aa, bb);

            if ((angle > ((linearSystemIndex + 90)/2))) {
                // print("wider");
                LinearWider("Ta");
            } else {
                // print("narrow");
                LinearNarrower("Ta");
            }
        }

        LinearCheck();
    }

    public void UpdateTarget () {
        GameObject newTarget = null;
        GameObject[] targetsFoundList = {};
        int i = stageIndex;
        if (targetTags[i] == "") {
            return;
        }
        targetsFoundList = GameObject.FindGameObjectsWithTag(targetTags[i]);
        float minDistance = 3.4E+38f;
        for (int j = 0; j < targetsFoundList.Length; j++) {
            float distance = Vector3.Distance(frontAnchor.transform.position, targetsFoundList[j].transform.position);
            if (j == 0) {
                minDistance = distance;
                newTarget = targetsFoundList[j];
            } else {
                if (distance < minDistance) {
                    minDistance = distance;
                    newTarget = targetsFoundList[j];
                }
            }
            // print("min" + minDistance);
        }
        if (newTarget != null) {
            targets[i] = newTarget;
        }
        if (!officeGame) {
            if (minDistance < targetRadius) {
                alertMode = true;
            } else {
                alertMode = false;
            }
        }
    }

    public void UpdateTargetName (int index, string name) {
        targetNames[index] = name;
        UpdateTarget();
    }

    public void UpdateTargetTag (int index, string tag) {
        targetTags[index] = tag;
        UpdateTarget();
    }

    void Moving() {
        idleFrame++;

        if (idleFrame > IdleFixationDurationList[sensIndex] * 72) {
            startToNarrow = true;
        }

        // idle for a while
        if (idleFrame > 1 && startToNarrow) {
            LinearWider("Id");
            idleFrame = 0;
        }

        // thumbstick movement
        if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero) {
            LinearNarrower("Id");
            startToNarrow = false;
            idleFrame = 0;
        }

        // heads movement
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

        if (distance > IdleHeadSensList[sensIndex]) {
            LinearNarrower("Id");
            startToNarrow = false;
            idleFrame = 0;
        }

        LinearCheck();
    }

    void Holistic() {
        UpdateTarget();

        if (InputBridge.Instance.BButton || InputBridge.Instance.RightThumbstickAxis != Vector2.zero) {
            manualLock = true;
        } else {
            manualLock = false;
        }

        if (manualLock) {
            LinearWider("Ma");
        } else {
            if (alertMode) {
                // Target
                var ta = frontAnchor.transform.position - mainCamera.transform.position;
                var tb = targets[stageIndex].transform.position - mainCamera.transform.position;

                var taa = new Vector2(ta.x, ta.z);
                var tbb = new Vector2(tb.x, tb.z);

                float tangle = Vector2.Angle(taa, tbb);
                // print("angle" + angle);

                if ((tangle > ((linearSystemIndex + 90)/2))) {
                    // print("taget not in sight");
                    if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero) {
                        // print("moving narrower");
                        LinearNarrower("Id");
                    } else {
                        // print("target wider");
                        LinearWider("Ta");
                    }
                } else {
                    // print("target in sight");
                    // print("narrow");
                    if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero) {
                        LinearNarrower("Id");
                    } else {
                        var a = frontAnchor.transform.position - mainCamera.transform.position;
                        var b = rightHandAnchor.transform.position - mainCamera.transform.position;
                        var c = leftHandAnchor.transform.position - mainCamera.transform.position;

                        var aa = new Vector2(a.x, a.z);
                        var bb = new Vector2(b.x, b.z);
                        var cc = new Vector2(c.x, c.z);

                        float angleRight = Vector2.Angle(aa, bb);
                        float angleLeft = Vector2.Angle(aa, cc);
                        // print("angleRight" + angleRight);
                        // print("angleLeft" + angleLeft);

                        // calculate hands distance
                        float handsDistance = Vector3.Distance(rightHandAnchor.transform.position, leftHandAnchor.transform.position);

                        if ( handsDistance > 1f && (angleRight > (linearSystemIndex + 55) || angleLeft > (linearSystemIndex + 55))) {
                            // print("hands wider");
                            LinearWider("Ha");
                        } else {
                            // print("hands narrow");
                            LinearNarrower("Ha");
                        }
                    }
                }
            } else {
                if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero) {
                    LinearNarrower("Id");
                } else {
                    var a = frontAnchor.transform.position - mainCamera.transform.position;
                    var b = rightHandAnchor.transform.position - mainCamera.transform.position;
                    var c = leftHandAnchor.transform.position - mainCamera.transform.position;

                    var aa = new Vector2(a.x, a.z);
                    var bb = new Vector2(b.x, b.z);
                    var cc = new Vector2(c.x, c.z);

                    float angleRight = Vector2.Angle(aa, bb);
                    float angleLeft = Vector2.Angle(aa, cc);
                    // print("angleRight" + angleRight);
                    // print("angleLeft" + angleLeft);

                    // calculate hands distance
                    float handsDistance = Vector3.Distance(rightHandAnchor.transform.position, leftHandAnchor.transform.position);

                    if ( handsDistance > 1f && (angleRight > (linearSystemIndex + 55) || angleLeft > (linearSystemIndex + 55))) {
                        // print("hands wider");
                        LinearWider("Ha");
                    } else {
                        // print("hands narrow");
                        LinearNarrower("Ha");
                    }
                }
            }
        }

        LinearCheck();
    }

    void LinearWider (string trigger = "") {
        if (linearSystemIndex == linearSystemLength - 1) {
            return;
        } else {
            if (log) {
                LogWriter.instance.ZoomTag("Out" + trigger);
            }
            linearSystemIndex++;
            OnWide.Invoke();
        }
    }

    void LinearNarrower (string trigger = "") {
        if (linearSystemIndex == 0) {
            return;
        } else {
            if (log) {
                LogWriter.instance.ZoomTag("In" + trigger);
            }
            linearSystemIndex--;
            OnNarrow.Invoke();
        }
    }

    void LinearCheck () {
        // for (int i = 0; i < linearSystem.Length; i++) {
        //     if (i == linearSystemIndex) {
        //         linearSystem[i].SetActive(true);
        //     } else {
        //         linearSystem[i].SetActive(false);
        //     }
        // }
    }

    public void SetAlertMode (bool status) {
        alertMode = status;
    }

    public void AddTargetRadius () {
        targetRadius += 1f;
        radiusText.text = targetRadius.ToString("F0");
    }

    public void MinusTargetRadius () {
        targetRadius -= 1f;
        radiusText.text = targetRadius.ToString("F0");
    }

    public void ResetTargetRadius () {
        radiusText.text = "[Default] 3";
        targetRadius = 3f;
    }
}
