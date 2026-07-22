using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using TMPro;
using BNG;

public class LevelTeleport : MonoBehaviour
{
    public static LevelTeleport instance;
    public Transform PuzzleDest;
    public Transform ShootingDest;
    public Transform MazeDest;
    public Transform CityDest;

    public TMP_Text stageValueText;

    public PlayerTeleport player;
    // public GameObject miniMap;
    public GameObject testPanel;
    public TMP_Text timeLeftText;

    [Header("/// GOD MODE ///")]
    public bool OpenTestPanel = false;
    public bool CloseTestPanel = false;
    public bool PlayComplete = false;
    public VideoPlayer completeVideo;
    public bool PlayNone = false;
    public VideoPlayer noneVideo;
    public bool PlayVision = false;
    public VideoPlayer visionVideo;
    [Header("---")]
    public bool StartGame = false;
    public bool StopGame = false;
    public bool ResetGame = false;
    public int TimeLeft = 0;
    [Header("---")]
    public bool SwitchNone = false;
    public bool SwitchHolistic = false;
    [Header("---")]
    public bool Recalibrate = false;
    int recalibrateCount = 0;
    [Header("---")]
    public int JumpToStage = 3;
    public bool ResetStage = false;
    public bool StartUserTest = false;
    public bool StopUserTest = false;
    public bool Dynamic = false;
    public int Method = 0;

    int frame = 0;
    // bool pressed = false;

    int currentIndex = 0;
    string[] stages = {"1 puzzle", "2 shooting", "3 maze", "4 game"};

    // Start is called before the first frame update
    void Start()
    {
        stageValueText.text = stages[currentIndex];
        // HideMiniMap();
        Recalibrate = true;
    }

    // Update is called once per frame
    void Update()
    {
        TimeLeft = int.Parse(timeLeftText.text);

        if (JumpToStage % stages.Length != currentIndex) {
            currentIndex = JumpToStage % stages.Length;
            JumpToStage = currentIndex;
            SwitchStage();
        }

        if (ResetStage) {
            ResetStage = false;
            ShuffleClusters.instance.Reset();
            GameControl.instance.ResetScore();
            GameControl.instance.ResetTimer();
        }

        if (ResetGame) {
            ResetGame = false;
            GameControl.instance.ResetScore();
            GameControl.instance.ResetTimer();
        }

        if (SwitchHolistic) {
            SwitchHolistic = false;
            Method = 5;
            Dynamic = true;
        }

        if (Recalibrate) {
            Method = 3;
            Dynamic = true;
            recalibrateCount++;
        }

        if (recalibrateCount >= 150 && Recalibrate) {
            Recalibrate = false;
            recalibrateCount = 0;
            Method = 5;
            Dynamic = true;
        }

        if (SwitchNone) {
            SwitchNone = false;
            ProCamControl.instance.MethodModeCheck(0);
            Method = 0;
            Dynamic = false;
        }

        if (StartGame) {
            StartGame = false;
            UserTest.instance.Play();
        }

        if (StopGame) {
            StopGame = false;
            UserTest.instance.Stop();
        }

        if (StartUserTest) {
            StartUserTest = false;
            UserTest.instance.Play();
        }

        if (StopUserTest) {
            StopUserTest = false;
            UserTest.instance.Stop();
        }

        if (CloseTestPanel) {
            CloseTestPanel = false;
            testPanel.SetActive(false);
            // GameControl.instance.TogglePointerCylinders(false);
        }

        if (OpenTestPanel) {
            OpenTestPanel = false;
            testPanel.SetActive(true);
            // GameControl.instance.TogglePointerCylinders(true);
        }

        if (PlayComplete) {
            PlayComplete = false;
            completeVideo.Play();
            noneVideo.Stop();
            visionVideo.Stop();
        }

        if (PlayNone) {
            PlayNone = false;
            completeVideo.Stop();
            noneVideo.Play();
            visionVideo.Stop();
        }

        if (PlayVision) {
            PlayVision = false;
            completeVideo.Stop();
            noneVideo.Stop();
            visionVideo.Play();
        }

        if (Dynamic) {
            ProCamControl.instance.InterfaceModeCheck(1);

            switch (Method) {
                case 0:
                    ProCamControl.instance.MethodModeCheck(0);
                    break;
                case 1:
                    ProCamControl.instance.MethodModeCheck(1);
                    break;
                case 2:
                    ProCamControl.instance.MethodModeCheck(2);
                    break;
                case 3:
                    ProCamControl.instance.MethodModeCheck(3);
                    break;
                case 4:
                    ProCamControl.instance.MethodModeCheck(4);
                    break;
                case 5:
                    ProCamControl.instance.MethodModeCheck(5);
                    break;
                default:
                    Method = 0;
                    break;
            }
        } else {
            ProCamControl.instance.InterfaceModeCheck(0);
        }
    }

    public void DynamicOn () {
        Dynamic = true;
    }

    public void DynamicOff () {
        Dynamic = false;
    }

    public void ChangeMethod (int method) {
        Method = method;
    }

    void SwitchStage() {
        stageValueText.text = stages[currentIndex];
        if (currentIndex == 0) {
            player.TeleportPlayerToTransform(PuzzleDest);
        } else if (currentIndex == 1) {
            player.TeleportPlayerToTransform(ShootingDest);
        } else if (currentIndex == 2) {
            player.TeleportPlayerToTransform(MazeDest);
        } else if (currentIndex == 3) {
            player.TeleportPlayerToTransform(CityDest);
        }
    }

    public void UpdateStageCurrentIndex(int index) {
        currentIndex = index;
        JumpToStage = currentIndex;
        SwitchStage();
    }

    public void NextStage() {
        currentIndex = (currentIndex + 1) % stages.Length;
        JumpToStage = currentIndex;
        SwitchStage();

        // if (currentIndex == 3) {
        //     ShowMiniMap();
        // } else {
        //     HideMiniMap();
        // }
    }

    public void PreviousStage() {
        if (currentIndex == 0) {
            currentIndex = stages.Length - 1;
        } else {
            currentIndex = (currentIndex - 1) % stages.Length;
        }
        JumpToStage = currentIndex;
        SwitchStage();

        // if (currentIndex == 3) {
        //     ShowMiniMap();
        // } else {
        //     HideMiniMap();
        // }
    }

    // public void ShowMiniMap() {
    //     // if (currentIndex == 3) {
    //     //     miniMap.SetActive(true);
    //     // }
    // }

    // public void HideMiniMap() {
    //     miniMap.SetActive(false);
    // }

    private void Awake() {
        instance = this;
    }
}
