using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.UI;

public class TableGameOffice : MonoBehaviour
{
    public static TableGameOffice instance;

    private void Awake() {
        instance = this;
    }

    public Text[] rings;

    private string[] blocks = { "mesh_props_03", "mesh_props_04", "mesh_props_05", "mesh_props_06" };

    private string[] blocksTag = { "Book03",
                                "Book04",
                                "Book05",
                                "Book06"};

    int index = 0;
    Color[] colors = new Color[4] {Color.yellow, Color.green, Color.blue, Color.red};

    public AudioSource correctSound;
    public AudioSource incorrectSound;

    // Start is called before the first frame update
    void Start()
    {
        Next();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DisableAll () {
        for (int i = 0; i < rings.Length; i++) {
            rings[i].color = new Color(1.0f, 0.92f, 0.016f, 1.0f);
        }
    }

    void ChangeAllColor (int index) {
        // change text color
        for (int i = 0; i < rings.Length; i++) {
            rings[i].color = colors[index];
        }
    }

    public void GetPoint(string snap) {
        print("snap: " + snap);
        if (snap.Contains(blocks[index])) {
            // play correct sound
            correctSound.Play();

            Next();

            // destory correct one
            SnapZoneTable0.instance.Destroy();
            SnapZoneTable1.instance.Destroy();
            SnapZoneTable2.instance.Destroy();
            SnapZoneTable3.instance.Destroy();
            SnapZoneTable4.instance.Destroy();
            SnapZoneTable5.instance.Destroy();

            GameControl.instance.AddScore(1);
        } else {
            // play incorrect sound
            incorrectSound.Play();

            // destory incorrect one
            SnapZoneTable0.instance.Destroy();
            SnapZoneTable1.instance.Destroy();
            SnapZoneTable2.instance.Destroy();
            SnapZoneTable3.instance.Destroy();
            SnapZoneTable4.instance.Destroy();
            SnapZoneTable5.instance.Destroy();
        }
    }

    void Next() {
        DisableAll();
        int old_index = index;
        while (index == old_index) {
            index = Random.Range(0, 4);
        }
        switch (index) {
            case 0:
                ChangeAllColor(0);
                break;
            case 1:
                ChangeAllColor(1);
                break;
            case 2:
                ChangeAllColor(2);
                break;
            case 3:
                ChangeAllColor(3);
                break;
            default:
                break;
        }

        // ProCamControl.instance.UpdateTargetTag(0, blocksTag[index]);
        // ProCamControl.instance.UpdateTargetName(0, blocks[index]);
    }
}
