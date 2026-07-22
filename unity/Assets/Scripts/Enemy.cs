using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;

    private void Awake() {
        instance = this;
    }

    public GameObject[] RoutesA;
    public GameObject EntityA;
    int nextIndexA = 1;
    bool attackA = false;
    public GameObject[] RoutesB;
    public GameObject EntityB;
    int nextIndexB = 1;
    bool attackB = false;
    public GameObject[] RoutesC;
    public GameObject EntityC;
    int nextIndexC = 1;
    bool attackC = false;
    public GameObject[] RoutesD;
    public GameObject EntityD;
    int nextIndexD = 1;
    bool attackD = false;
    bool on = false;

    public GameObject player;
    float attackRadius = 5f;

    public AudioSource attackSound;

    bool[] playSound = {false, false, false, false};
    bool[] soundPlayed = {false, false, false, false};
    bool[] alive = {true, true, true, true};

    // Start is called before the first frame update
    void Start()
    {
        EntityA.transform.position = RoutesA[0].transform.position;
        EntityB.transform.position = RoutesB[0].transform.position;
        EntityC.transform.position = RoutesC[0].transform.position;
        EntityD.transform.position = RoutesD[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is in attack range
        if (alive[0] && Vector2.Distance(new Vector2(EntityA.transform.position.x, EntityA.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < attackRadius && player.transform.position.y > 0f) {
            attackA = true;
            PlaySound(0);
        } else {
            attackA = false;
            soundPlayed[0] = false;
        }
        if (alive[1] && Vector2.Distance(new Vector2(EntityB.transform.position.x, EntityB.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < attackRadius && player.transform.position.y > 0f) {
            attackB = true;
            PlaySound(1);
        } else {
            attackB = false;
            soundPlayed[1] = false;
        }
        if (alive[2] && Vector2.Distance(new Vector2(EntityC.transform.position.x, EntityC.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < attackRadius && player.transform.position.y > 0f) {
            attackC = true;
            PlaySound(2);
        } else {
            attackC = false;
            soundPlayed[2] = false;
        }
        if (alive[3] && Vector2.Distance(new Vector2(EntityD.transform.position.x, EntityD.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < attackRadius && player.transform.position.y > 0f) {
            attackD = true;
            PlaySound(3);
        } else {
            attackD = false;
            soundPlayed[3] = false;
        }

        if (attackA || attackB || attackC || attackD) {
            ProCamControl.instance.SetAlertMode(true);
        } else {
            ProCamControl.instance.SetAlertMode(false);
        }

        if (attackA) {
            EntityA.transform.position = Vector3.MoveTowards(EntityA.transform.position, player.transform.position, 0.01f);
        } else {
            EntityA.transform.position = Vector3.MoveTowards(EntityA.transform.position, RoutesA[nextIndexA].transform.position, 0.01f);
            if (EntityA.transform.position == RoutesA[nextIndexA].transform.position) {
                nextIndexA = (nextIndexA + 1) % RoutesA.Length;
            }
        }
        if (attackB) {
            EntityB.transform.position = Vector3.MoveTowards(EntityB.transform.position, player.transform.position, 0.01f);
        } else {
            EntityB.transform.position = Vector3.MoveTowards(EntityB.transform.position, RoutesB[nextIndexB].transform.position, 0.01f);
            if (EntityB.transform.position == RoutesB[nextIndexB].transform.position) {
                nextIndexB = (nextIndexB + 1) % RoutesB.Length;
            }
        }
        if (attackC) {
            EntityC.transform.position = Vector3.MoveTowards(EntityC.transform.position, player.transform.position, 0.01f);
        } else {
            EntityC.transform.position = Vector3.MoveTowards(EntityC.transform.position, RoutesC[nextIndexC].transform.position, 0.01f);
            if (EntityC.transform.position == RoutesC[nextIndexC].transform.position) {
                nextIndexC = (nextIndexC + 1) % RoutesC.Length;
            }
        }
        if (attackD) {
            EntityD.transform.position = Vector3.MoveTowards(EntityD.transform.position, player.transform.position, 0.01f);
        } else {
            EntityD.transform.position = Vector3.MoveTowards(EntityD.transform.position, RoutesD[nextIndexD].transform.position, 0.01f);
            if (EntityD.transform.position == RoutesD[nextIndexD].transform.position) {
                nextIndexD = (nextIndexD + 1) % RoutesD.Length;
            }
        }
    }

    void PlaySound (int index) {
        foreach (bool played in soundPlayed) {
            if (played) {
                return;
            }
        }
        attackSound.Play();
        soundPlayed[index] = true;
    }

    public void SetAlive (int index) {
        alive[index] = true;
    }

    public void SetDead (int index) {
        alive[index] = false;
    }

    public void TurnOn() {
        on = true;
    }

    public void TurnOff() {
        on = false;
    }
}
