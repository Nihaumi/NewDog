using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //dog
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("GermanShepherd_Prefab");
        //target = GameObject.Find("to_follow");

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .03f);
    }
}
