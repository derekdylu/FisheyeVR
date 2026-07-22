using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BNG;

public class LogWriter : MonoBehaviour
{
    public static LogWriter instance;
    private bool logger = false;
    public GameObject player;
    public GameObject head;
    public GameObject rightHand;
    public GameObject leftHand;

    public string experimentType;
    public string user;
    public string number;
    public TMP_Text[] configs;
    private string[] categories = {"stage:", "beginScore:", "mode:", "fov:", "method:", "interaxial:", "sensitivity:"};

    private string _timestamp = "errorGettingTimestamp";
    private string docPath = "errorGettingDirectoryPath";
    private string fileName = "errorGettingFileName";
    private int frame = 0;
    private int updateRate = 5;
    private float fixedUpdateRate = 0.0133338f;
    public bool IsPilot = true;
    private static readonly Regex ParticipantIdPattern = new Regex(
        @"^P[0-9]{3,6}$",
        RegexOptions.CultureInvariant
    );
    private static readonly Regex FileTokenPattern = new Regex(
        @"^[A-Za-z0-9_-]{1,32}$",
        RegexOptions.CultureInvariant
    );

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }

    public void ToggleLogger()
    {
        if (logger) {
            StopLogger();
        } else {
            StartLogger();
        }
    }

    public void StartLogger()
    {
        string participantId = ValidateParticipantId(user);
        string experimentId = ValidateFileToken(experimentType, "Experiment type");
        docPath = Path.Combine(Application.persistentDataPath, "ResearchLogs");
        Directory.CreateDirectory(docPath);

        if (IsPilot) {
            logger = true;
            DateTime timestamp = DateTime.Now;
            fileName =  "LOG_" +
                        experimentId + "_" + // pilot1 ... formal
                        participantId + "_" +
                        // configs[0].text.Substring(2) + "_" + // stage
                        // configs[2].text + "_" + // static / dynamic
                        // configs[4].text + "_" + // method
                        // configs[6].text + "_" + // sensitivity
                        UnityEngine.Random.Range(0, 1000).ToString() + // random number
                        // number + // test number
                        ".txt";

            Debug.Log("experiment log started at: " + timestamp.ToString());
            Debug.Log("log file saved in: " + docPath);
            Debug.Log("with file name: " + fileName);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName)))
            {
                outputFile.WriteLine("startedTime:" + timestamp);
                outputFile.WriteLine("user:" + participantId);
                // for (int i = 0; i < configs.Length; i++)
                // {
                //     outputFile.WriteLine(categories[i] + configs[i].text);
                // }
                outputFile.WriteLine("frameLength:" + updateRate * fixedUpdateRate);
            }
        } else {
            GameControl.instance.StartLogger();
            logger = true;
            DateTime timestamp = DateTime.Now;
            fileName =  "LOG_" +
                        experimentId + "_" + // pilot1 ... formal
                        participantId + "_" +
                        // configs[0].text.Substring(2) + "_" + // stage
                        configs[2].text + "_" + // static / dynamic
                        // configs[4].text + "_" + // method
                        // configs[6].text + "_" + // sensitivity
                        UnityEngine.Random.Range(0, 100).ToString() + // random number
                        // number + // test number
                        ".txt";

            Debug.Log("experiment log started at: " + timestamp.ToString());
            Debug.Log("log file saved in: " + docPath);
            Debug.Log("with file name: " + fileName);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName)))
            {
                outputFile.WriteLine("startedTime:" + timestamp);
                outputFile.WriteLine("user:" + participantId);
                for (int i = 0; i < configs.Length; i++)
                {
                    outputFile.WriteLine(categories[i] + configs[i].text);
                }
                outputFile.WriteLine("frameLength:" + updateRate * fixedUpdateRate);
            }
        }
    }

    private static string ValidateParticipantId(string value)
    {
        string participantId = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (!ParticipantIdPattern.IsMatch(participantId)) {
            throw new InvalidOperationException(
                "Participant ID must use the form P001 (P followed by 3-6 digits)."
            );
        }
        return participantId;
    }

    private static string ValidateFileToken(string value, string label)
    {
        string token = (value ?? string.Empty).Trim();
        if (!FileTokenPattern.IsMatch(token)) {
            throw new InvalidOperationException(
                label + " must contain only letters, digits, underscores, or hyphens."
            );
        }
        return token;
    }

    public void StopLogger()
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName), true))
        {
            outputFile.WriteLine("===");
            outputFile.WriteLine("endedTime:" + DateTime.Now);
            outputFile.WriteLine("finalScore:" + configs[1].text);
        }
        GameControl.instance.StopLogger();
        logger = false;
    }

    public void Tag(string tag)
    {
        if (!logger) {
            return;
        }
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName), true))
        {
            outputFile.WriteLine("*" + tag);
        }
    }

    public void ZoomTag(string zoomTag)
    {
        if (!logger) {
            return;
        }
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName), true))
        {
            outputFile.WriteLine("#" + zoomTag);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (logger) {
            frame++;
            if (frame == updateRate) {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, fileName), true))
                {
                    outputFile.WriteLine(
                                            head.transform.rotation.ToString() + '%' +
                                            head.transform.eulerAngles.ToString() + '%' +
                                            rightHand.transform.position.ToString() + '%' +
                                            leftHand.transform.position.ToString() + '%' +
                                            rightHand.transform.rotation.ToString() + '%' +
                                            leftHand.transform.rotation.ToString() + '%' +
                                            rightHand.transform.eulerAngles.ToString() + '%' +
                                            leftHand.transform.eulerAngles.ToString() + '%' +
                                            player.transform.position.ToString()
                    );

                }
                frame = 0;
            }
        }
    }

    private void Awake() {
        instance = this;
    }
}
