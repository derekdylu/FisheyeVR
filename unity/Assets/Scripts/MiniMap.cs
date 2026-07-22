using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject playerPosition;
    public GameObject playerAngle;
    public GameObject indicator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        indicator.transform.localPosition = new Vector3(-180f+(playerPosition.transform.localPosition.x - (-25f))*360f/30f, -230f+(playerPosition.transform.localPosition.z-10f)*460f/41f, 0f);
        indicator.transform.localRotation = Quaternion.Euler(0f, 0f, -playerAngle.transform.eulerAngles.y);
    }
}
