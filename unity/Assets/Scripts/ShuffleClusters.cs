using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using Random = System.Random;

public class ShuffleClusters : MonoBehaviour
{
    public static ShuffleClusters instance;

    private void Awake() {
        instance = this;
    }

    public GameObject[] clusters;
    List<GameObject> initiatedClusters = new List<GameObject>();
    List<GameObject> initiatedCityCrates = new List<GameObject>();
    public GameObject[] cityCrates;
    public GameObject[] targetIndicators;
    private GameObject[] targets;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBlocks();
        // Reset();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < targetIndicators.Length; i++) {
            GameObject crate = targets[i].transform.GetChild(0).gameObject;
            if (crate.activeSelf == true) {
                targetIndicators[i].SetActive(true);
                targetIndicators[i].transform.localPosition = new Vector3(-180f+(targets[i].transform.position.x - (19f))*360f/30f, -230f+(targets[i].transform.position.z - 35f)*460f/41f, 0f);
            } else {
                targetIndicators[i].SetActive(false);
            }
        }
    }

    public void GenerateMoreBlocks() {
        GenerateBlocks();
    }

    public void Reset() {
        int cnt = initiatedClusters.Count;
        for (int i = cnt - 1; i >= 0; i--) {
            Destroy(initiatedClusters[i]);
        }
        int cnt2 = initiatedCityCrates.Count;
        for (int i = cnt2 - 1; i >= 0; i--) {
            Destroy(initiatedCityCrates[i]);
        }
        initiatedClusters.Clear();
        initiatedCityCrates.Clear();
        GenerateBlocks();
    }

    void GenerateBlocks() {
        int section;
        float x;
        float y;

        // table
        for (int i = 0; i < clusters.Length; i++) {
            for (int j = 0; j < 20; j++) {
                section = Random.Range(0, 3);
                // print(section);
                if (section == 0) {
                    x = Random.Range(39f, 39.6f);
                    y = Random.Range(38f, 39.2f);
                } else if (section == 1) {
                    x = Random.Range(38.65f, 41.37f);
                    y = Random.Range(37f, 37.5f);
                } else if (section == 2) {
                    x = Random.Range(40.38f, 40.77f);
                    y = Random.Range(38f, 39.2f);
                } else {
                    return;
                }

                GameObject obj = (GameObject)Instantiate(clusters[i], new Vector3(x,0.05f,y), Quaternion.identity);
                initiatedClusters.Add(obj);
            }
        }

        // city
        for (int i = 0; i < cityCrates.Length; i++) {
            for (int j = 0; j < 10; j++) {
                x = Random.Range(14.8f, 44.6f);
                y = Random.Range(47.9f, 88.2f);
                while (CheckCityCratePos(x, y)) {
                    x = Random.Range(14.8f, 44.6f);
                    y = Random.Range(47.9f, 88.2f);
                }
                GameObject obj2 = (GameObject)Instantiate(cityCrates[i], new Vector3(x, 18f ,y), Quaternion.identity);
                initiatedCityCrates.Add(obj2);
            }
        }

        targets = initiatedCityCrates.ToArray();
    }

    bool CheckCityCratePos (float x, float y) {
        if (((x > 19f && x < 23f) || (x > 28f && x < 32f) || (x > 37.4f && x < 41.4f)) && ((y > 53.4f && y < 57.4f) || (y > 61.4f && y < 65.4f) || (y > 70.4f && y < 74.4f) || (y > 78.4f && y < 82.4f))) {
            return true;
        } else {
            return false;
        }
    }
}
