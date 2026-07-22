using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class TableGame : MonoBehaviour
{
    public static TableGame instance;

    private void Awake() {
        instance = this;
    }

    public GameObject orange;
    public GameObject yellow;
    public GameObject pink;
    public GameObject teal;
    public GameObject white;

    private string[] blocks = { "BlockOrange(Clone)",
                                "BlockYellow(Clone)",
                                "BlockPink(Clone)",
                                "BlockTeal(Clone)",
                                "BlockWhite(Clone)"};

    private string[] blocksTag = { "BlockOrange",
                                "BlockYellow",
                                "BlockPink",
                                "BlockTeal",
                                "BlockWhite"};

    int index = 0;

    public AudioSource correctSound;
    public AudioSource incorrectSound;

    // Start is called before the first frame update
    void Start()
    {
        orange.SetActive(false);
        yellow.SetActive(false);
        pink.SetActive(false);
        teal.SetActive(false);
        white.SetActive(false);
        Next();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DisableAll () {
        orange.SetActive(false);
        yellow.SetActive(false);
        pink.SetActive(false);
        teal.SetActive(false);
        white.SetActive(false);
    }

    public void GetPoint(string snap) {
        if (snap == blocks[index]) {
            // play correct sound
            correctSound.Play();

            Next();

            // destory correct one
            SnapZoneTable.instance.Destroy();

            GameControl.instance.AddScore(1);
        } else {
            // play incorrect sound
            incorrectSound.Play();

            // destory incorrect one
            SnapZoneTable.instance.Destroy();
        }
    }

    void Next() {
        DisableAll();
        int old_index = index;
        while (index == old_index) {
            index = Random.Range(0, 5);
        }
        switch (index) {
            case 0:
                orange.SetActive(true);
                break;
            case 1:
                yellow.SetActive(true);
                break;
            case 2:
                pink.SetActive(true);
                break;
            case 3:
                teal.SetActive(true);
                break;
            case 4:
                white.SetActive(true);
                break;
            default:
                break;
        }

        ProCamControl.instance.UpdateTargetTag(0, blocksTag[index]);
        ProCamControl.instance.UpdateTargetName(0, blocks[index]);
    }
}
