using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;
using TMPro;
using BNG;
using UnityEngine.Events;

public class PilotStudy : MonoBehaviour
{
    public static PilotStudy instance;
    // public UnityEvent OnSetLensIndex;
    public Transform PuzzleDest;
    public Transform ShootingDest;
    public Transform MazeDest;
    public Transform CityDest;

    // public TMP_Text stageValueText;

    public PlayerTeleport player;
    // public GameObject miniMap;
    // public GameObject testPanel;
    // public TMP_Text timeLeftText;

    [Header("/// GOD MODE ///")]
    // public bool OpenTestPanel = false;
    // public bool CloseTestPanel = false;
    // public bool PlayComplete = false;
    // public VideoPlayer completeVideo;
    // public bool PlayNone = false;
    // public VideoPlayer noneVideo;
    // public bool PlayVision = false;
    // public VideoPlayer visionVideo;
    [Header("---")]
    // public bool StartGame = false;
    // public bool StopGame = false;
    // public bool ResetGame = false;
    // public int TimeLeft = 0;
    [Header("---")]
    // public bool SwitchNone = false;
    // public bool SwitchHolistic = false;
    [Header("---")]
    // public bool Recalibrate = false;
    // int recalibrateCount = 0;
    [Header("---")]
    [Header("/// PILOT(RE) USER TEST ///")]
    [Header("Stage Control (not logged, don't use when doing user test)")]
    public int JumpToStage = 0;
    public bool PreviousStage = false;
    public bool NextStage = false;
    public bool ResetStage = false;
    [Header("User Test")]
    public bool StartUserTest = false;
    public bool ForceExit = false;
    public bool StopUserTest = false;
    public bool ResetUserTest = false;
    // public bool Dynamic = false;
    [Header("---")]
    // public int Method = 0;

    public int maxTime = 5;
    public int timeRemain = 5;

    int frame = 0;
    // bool pressed = false;

    int currentIndex = 0;
    int lensIndex = 0;
    public int FOVValue = 90;
    int[] FOVValueList = {90, 110, 130, 150, 170};
    string[] stages = {"1 puzzle", "2 shooting", "3 maze", "4 game"};
    int[] FOVIndexList = {26, 6, 4, 2, 1};
    int[] stageOrder = {0,1,2,3};

    int round = 0;
    int stageIndex = 0;

    bool start = false;

    int[] RandomStageOrder() {
        int[] randomOrder = new int[stages.Length];
        for (int i = 0; i < stages.Length; i++) {
            randomOrder[i] = i;
        }
        for (int i = 0; i < stages.Length; i++) {
            int temp = randomOrder[i];
            int randomIndex = Random.Range(i, stages.Length);
            randomOrder[i] = randomOrder[randomIndex];
            randomOrder[randomIndex] = temp;
        }
        return randomOrder;
    }

    void FixedUpdate() {
        if (start) {
            if (ForceExit) {
                ForceExit = false;
                LogWriter.instance.Tag("FORCE_EXIT: " + FOVValueList[lensIndex].ToString());
                LogWriter.instance.StopLogger();
                start = false;
            }
            frame++;
            if (frame % 60 == 0) {
                timeRemain--;
                if (timeRemain <= 0) {
                    timeRemain = maxTime;
                    if (round < 3) {
                        round++;
                    } else {
                        round = 0;
                        lensIndex++;
                        if (lensIndex >= FOVIndexList.Length) {
                            start = false;
                        }
                        LogWriter.instance.Tag("===^===");
                    }
                    CameraBody.instance.SetLensIndex(FOVIndexList[lensIndex]);
                    FOVValue = FOVValueList[lensIndex];
                    LogWriter.instance.Tag("Lens: " + FOVValueList[lensIndex].ToString());
                    currentIndex = stageOrder[round];
                    LogWriter.instance.Tag("Stage: " + stages[currentIndex]);
                    ResetStage = true;
                    SwitchStage();

                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentIndex = stageOrder[stageIndex];
        SwitchStage();
        // stageValueText.text = stages[currentIndex];
        // HideMiniMap();
        // Recalibrate = true;
    }

    // Update is called once per frame
    void Update()
    {
        // TimeLeft = int.Parse(timeLeftText.text);

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

        // if (ResetGame) {
        //     ResetGame = false;
        //     GameControl.instance.ResetScore();
        //     GameControl.instance.ResetTimer();
        // }

        // if (SwitchHolistic) {
        //     SwitchHolistic = false;
        //     Method = 5;
        //     Dynamic = true;
        // }

        // if (Recalibrate) {
        //     Method = 3;
        //     Dynamic = true;
        //     recalibrateCount++;
        // }

        // if (recalibrateCount >= 150 && Recalibrate) {
        //     Recalibrate = false;
        //     recalibrateCount = 0;
        //     Method = 5;
        //     Dynamic = true;
        // }

        // if (SwitchNone) {
        //     SwitchNone = false;
        //     ProCamControl.instance.MethodModeCheck(0);
        //     Method = 0;
        //     Dynamic = false;
        // }

        // if (StartGame) {
        //     StartGame = false;
        //     UserTest.instance.Play();
        // }

        // if (StopGame) {
        //     StopGame = false;
        //     UserTest.instance.Stop();
        // }

        // if (SkipStage) {
        //     SkipStage = false;
        //     timeRemain = 1;
        //     LogWriter.instance.Tag("FORCE_SKIP");
        // }

        if (StartUserTest) {
            stageOrder = RandomStageOrder();
            LogWriter.instance.StartLogger();
            StartUserTest = false;
            // UserTest.instance.Play();
            start = true;
            // LogWriter.instance.Tag("----------");
            CameraBody.instance.SetLensIndex(FOVIndexList[lensIndex]);
            LogWriter.instance.Tag("Lens: " + FOVValueList[lensIndex].ToString());
            currentIndex = stageOrder[round];
            LogWriter.instance.Tag("Stage: " + stages[currentIndex]);
            ResetStage = true;
            SwitchStage();
            FOVValue = FOVValueList[lensIndex];
        }

        if (StopUserTest) {
            LogWriter.instance.StopLogger();
            StopUserTest = false;
            // UserTest.instance.Stop();
            start = false;
        }

        if (ResetUserTest) {
            LogWriter.instance.StopLogger();
            ResetUserTest = false;
            ResetStage = true;
            // UserTest.instance.Stop();
            start = false;
        }

        if (NextStage) {
            NextStage = false;
            currentIndex++;
            if (currentIndex >= stages.Length) {
                currentIndex = 0;
            }
            ResetStage = true;
            SwitchStage();
        }

        if (PreviousStage) {
            PreviousStage = false;
            currentIndex--;
            if (currentIndex < 0) {
                currentIndex = stages.Length - 1;
            }
            ResetStage = true;
            SwitchStage();
        }

        // if (CloseTestPanel) {
            // CloseTestPanel = false;
            // testPanel.SetActive(false);
            // GameControl.instance.TogglePointerCylinders(false);
        // }

        // if (OpenTestPanel) {
        //     OpenTestPanel = false;
        //     testPanel.SetActive(true);
        //     // GameControl.instance.TogglePointerCylinders(true);
        // }

        // if (PlayComplete) {
        //     PlayComplete = false;
        //     completeVideo.Play();
        //     noneVideo.Stop();
        //     visionVideo.Stop();
        // }

        // if (PlayNone) {
        //     PlayNone = false;
        //     completeVideo.Stop();
        //     noneVideo.Play();
        //     visionVideo.Stop();
        // }

        // if (PlayVision) {
        //     PlayVision = false;
        //     completeVideo.Stop();
        //     noneVideo.Stop();
        //     visionVideo.Play();
        // }

        // if (Dynamic) {
        //     ProCamControl.instance.InterfaceModeCheck(1);

        //     switch (Method) {
        //         case 0:
        //             ProCamControl.instance.MethodModeCheck(0);
        //             break;
        //         case 1:
        //             ProCamControl.instance.MethodModeCheck(1);
        //             break;
        //         case 2:
        //             ProCamControl.instance.MethodModeCheck(2);
        //             break;
        //         case 3:
        //             ProCamControl.instance.MethodModeCheck(3);
        //             break;
        //         case 4:
        //             ProCamControl.instance.MethodModeCheck(4);
        //             break;
        //         case 5:
        //             ProCamControl.instance.MethodModeCheck(5);
        //             break;
        //         default:
        //             Method = 0;
        //             break;
        //     }
        // } else {
        //     ProCamControl.instance.InterfaceModeCheck(0);
        // }
    }

    // public void DynamicOn () {
    //     Dynamic = true;
    // }

    // public void DynamicOff () {
    //     Dynamic = false;
    // }

    // public void ChangeMethod (int method) {
    //     Method = method;
    // }

    void SwitchStage() {
        // stageValueText.text = stages[currentIndex];
        if (currentIndex == 0) {
            player.TeleportPlayerToTransform(PuzzleDest);
        } else if (currentIndex == 1) {
            player.TeleportPlayerToTransform(ShootingDest);
        } else if (currentIndex == 2) {
            player.TeleportPlayerToTransform(MazeDest);
        } else if (currentIndex == 3) {
            player.TeleportPlayerToTransform(CityDest);
        }
        JumpToStage = currentIndex;
    }

    // public void UpdateStageCurrentIndex(int index) {
    //     currentIndex = index;
    //     JumpToStage = currentIndex;
    //     SwitchStage();
    // }

    // public void NextStage() {
    //     currentIndex = (currentIndex + 1) % stages.Length;
    //     JumpToStage = currentIndex;
    //     SwitchStage();

    //     // if (currentIndex == 3) {
    //     //     ShowMiniMap();
    //     // } else {
    //     //     HideMiniMap();
    //     // }
    // }

    // public void PreviousStage() {
    //     if (currentIndex == 0) {
    //         currentIndex = stages.Length - 1;
    //     } else {
    //         currentIndex = (currentIndex - 1) % stages.Length;
    //     }
    //     JumpToStage = currentIndex;
    //     SwitchStage();

    //     // if (currentIndex == 3) {
    //     //     ShowMiniMap();
    //     // } else {
    //     //     HideMiniMap();
    //     // }
    // }

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
