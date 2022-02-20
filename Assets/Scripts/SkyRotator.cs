using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyRotator : MonoBehaviour
{
    public float rotation_speed = 0.25f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotation_speed);
    }
}
