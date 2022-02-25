using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_player_position : MonoBehaviour
{
    GameObject player;
    Vector3 player_position;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("CenterEyeAnchor");
        target = this.gameObject;
        target_pos = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       /* if (IsPlayerMoving())
        {
            SetPosition();
        }*/
        SetPosition();
    }

    void SetPosition()
    {
        player_position = player.transform.position;
        this.transform.position = new Vector3(player_position.x, transform.position.y, player_position.z);
    }

    GameObject target;
    Vector3 target_pos;
   [SerializeField] float distance;

    bool IsPlayerMoving()
    {
        distance = Vector3.Distance(target.transform.position, target_pos);
        target_pos = target.transform.position;

        if (distance > 0.3f)
        {
            return true;
        }
        else return false;
    }


   
}
