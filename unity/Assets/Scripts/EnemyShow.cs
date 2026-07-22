using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShow : MonoBehaviour
{
    public static EnemyShow instance;

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

    public GameObject[] ActivationPoins;

    bool[] playSound = {false, false, false, false};
    bool[] soundPlayed = {false, false, false, false};
    bool[] alive = {false, false, false, false};
    // Start is called before the first frame update

    bool[] died = {false, false, false, false};

    float activateRadius = 0.5f;

    void Start()
    {
        EntityA.transform.position = RoutesA[0].transform.position;
        EntityB.transform.position = RoutesB[0].transform.position;
        EntityC.transform.position = RoutesC[0].transform.position;
        EntityD.transform.position = RoutesD[0].transform.position;
        EntityA.SetActive(false);
        EntityB.SetActive(false);
        EntityC.SetActive(false);
        EntityD.SetActive(false);
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
        print("attacking" + attackA + " " + attackB + " " + attackC + " " + attackD);

        if (attackA || attackB || attackC || attackD) {
            ProCamControl.instance.SetAlertMode(true);
        } else {
            ProCamControl.instance.SetAlertMode(false);
        }

        if (!died[0] && Vector2.Distance(new Vector2(ActivationPoins[0].transform.position.x, ActivationPoins[0].transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < activateRadius && player.transform.position.y > 0f) {
            EntityA.SetActive(true);
            alive[0] = true;
        }
        if (!died[1] && Vector2.Distance(new Vector2(ActivationPoins[1].transform.position.x, ActivationPoins[1].transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < activateRadius && player.transform.position.y > 0f) {
            EntityB.SetActive(true);
            alive[1] = true;
        }
        if (!died[2] && Vector2.Distance(new Vector2(ActivationPoins[2].transform.position.x, ActivationPoins[2].transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < activateRadius && player.transform.position.y > 0f) {
            EntityC.SetActive(true);
            alive[2] = true;
        }
        if (!died[3] && Vector2.Distance(new Vector2(ActivationPoins[3].transform.position.x, ActivationPoins[3].transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) < activateRadius && player.transform.position.y > 0f) {
            EntityD.SetActive(true);
            alive[3] = true;
        }

        if (alive[0]) {
            EntityA.transform.position = Vector3.MoveTowards(EntityA.transform.position, RoutesA[nextIndexA].transform.position, 0.01f);
            if (EntityA.transform.position == RoutesA[nextIndexA].transform.position) {
                nextIndexA = (nextIndexA + 1) % RoutesA.Length;
            }
        }

        if (alive[1]) {
            EntityB.transform.position = Vector3.MoveTowards(EntityB.transform.position, RoutesB[nextIndexB].transform.position, 0.01f);
            if (EntityB.transform.position == RoutesB[nextIndexB].transform.position) {
                nextIndexB = (nextIndexB + 1) % RoutesB.Length;
            }
        }

        if (alive[2]) {
            EntityC.transform.position = Vector3.MoveTowards(EntityC.transform.position, RoutesC[nextIndexC].transform.position, 0.01f);
            if (EntityC.transform.position == RoutesC[nextIndexC].transform.position) {
                nextIndexC = (nextIndexC + 1) % RoutesC.Length;
            }
        }

        if (alive[3]) {
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

    public void SetDied (int index) {
        died[index] = true;
    }
}
