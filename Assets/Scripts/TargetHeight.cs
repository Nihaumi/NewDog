using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHeight : MonoBehaviour
{
    GameObject player;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("OVRCameraRig (1)");
        target = GameObject.Find("target");
    }

    // Update is called once per frame
    void Update()
    {
        SetTargetHeight();
    }

    void SetTargetHeight()
    {
        target.transform.position = new Vector3(target.transform.position.x, 0, target.transform.position.z);
    }
}
