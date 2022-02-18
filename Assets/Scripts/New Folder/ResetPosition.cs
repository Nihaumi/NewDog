using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    GameObject dog_child;
    GameObject dog;

    // Start is called before the first frame update
    void Start()
    {
        dog_child = GameObject.Find("DOg");
        dog = GameObject.Find("GermanShepherd_Prefab");
    }

    // Update is called once per frame
    void Update()
    {
        if(dog_child.transform.position != dog.transform.position)
        {
            dog_child.transform.position = dog.transform.position;
        }
    }
}
