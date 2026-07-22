using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using BNG;

public class ProCamControlSimple : MonoBehaviour
{
    public static ProCamControlSimple instance;

    private void Awake() {
        instance = this;
    }

    bool manualLock = false;

    public GameObject rightHandAnchor;
    public GameObject leftHandAnchor;
    public GameObject mainCamera;

    public GameObject targets = null;
    public string targetTags;
    public GameObject frontAnchor;
    public float targetRadius = 3f;

    // linear system
    public GameObject[] linearSystem;
    int linearSystemIndex = 0;

    bool alertMode = false;
    bool recalibrated = false;
    int frame = 0;
    int intervalFrame = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (recalibrated) {
            Holistic();
        } else {
            if (frame > 200) {
                recalibrated = true;
            }
            if (frame < 100) {
                if (intervalFrame > 5) {
                    LinearWider();
                    intervalFrame = 0;
                }
            } else {
                if (intervalFrame > 5) {
                    LinearNarrower();
                    intervalFrame = 0;
                }
            }
            LinearCheck();
            frame++;
            intervalFrame++;
        }
    }

    public void Recalibrate () {

    }

    public void UpdateTarget () {
        GameObject newTarget = null;
        GameObject[] targetsFoundList = {};
        if (targetTags == "") {
            return;
        }
        targetsFoundList = GameObject.FindGameObjectsWithTag(targetTags);
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
        }
        if (newTarget != null) {
            targets = newTarget;
        }
        if (minDistance < targetRadius) {
            alertMode = true;
        } else {
            alertMode = false;
        }
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
                var ta = frontAnchor.transform.position - mainCamera.transform.position;
                var tb = targets.transform.position - mainCamera.transform.position;

                var taa = new Vector2(ta.x, ta.z);
                var tbb = new Vector2(tb.x, tb.z);

                float tangle = Vector2.Angle(taa, tbb);

                if ((tangle > ((linearSystemIndex + 90)/2))) {
                    if (InputBridge.Instance.LeftThumbstickAxis != Vector2.zero) {
                        LinearNarrower("Id");
                    } else {
                        LinearWider("Ta");
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
                        float handsDistance = Vector3.Distance(rightHandAnchor.transform.position, leftHandAnchor.transform.position);

                        if ( handsDistance < 0.75f || angleRight < (linearSystemIndex + 55) || angleLeft < (linearSystemIndex + 55)) {
                            LinearNarrower("Ha");
                        } else {
                            LinearWider("Ha");
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
                    float handsDistance = Vector3.Distance(rightHandAnchor.transform.position, leftHandAnchor.transform.position);

                    if ( handsDistance < 0.75f || angleRight < (linearSystemIndex + 55) || angleLeft < (linearSystemIndex + 55)) {
                        LinearNarrower("Ha");
                    } else {
                        LinearWider("Ha");
                    }
                }
            }
        }

        LinearCheck();
    }

    void LinearWider (string trigger = "") {
        if (linearSystemIndex == linearSystem.Length - 1) {
            return;
        } else {
            linearSystemIndex++;
        }
    }

    void LinearNarrower (string trigger = "") {
        if (linearSystemIndex == 0) {
            return;
        } else {
            linearSystemIndex--;
        }
    }

    void LinearCheck () {
        for (int i = 0; i < linearSystem.Length; i++) {
            if (i == linearSystemIndex) {
                linearSystem[i].SetActive(true);
            } else {
                linearSystem[i].SetActive(false);
            }
        }
    }

    public void SetAlertMode (bool status) {
        alertMode = status;
    }
}
