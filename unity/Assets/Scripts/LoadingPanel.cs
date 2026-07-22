using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public GameObject loadingPanel;
    int frame = 0;

    // Start is called before the first frame update
    void Start()
    {
        loadingPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        if (frame >= 200) {
            loadingPanel.SetActive(false);
        }
    }
}
