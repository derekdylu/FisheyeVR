using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using BNG;

public class ScoreHelper : MonoBehaviour
{
    public static ScoreHelper instance;

    public TMP_Text scoreText;
    int shootingScore = 0;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        renderScores();
    }

    void Update()
    {
        if (InputBridge.Instance.AButton) {
            shootingScore = 0;
        }
    }

    public void ResetScore() {
        shootingScore = 0;
        renderScores();
    }

    public void ShootingAddPoint() {
        shootingScore++;
        renderScores();
    }

    void renderScores() {
        scoreText.text = "shootingScore: " + shootingScore.ToString();
    }
}
