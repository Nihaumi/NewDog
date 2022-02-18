using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //other script
    public GameObject dog;
    Basic_Behaviour basic_behav;

    //targets
    GameObject aim_hand_right;
    GameObject aim_hand_left;
    GameObject aim_head;
    public GameObject follow_left;
    public GameObject follow_right;

    //target obj
    GameObject Player_eyes;
    public GameObject hand_left;
    public GameObject hand_right;

    //hand positions
    Vector3 hand_left_position;
    Vector3 hand_right_position;
    // Start is called before the first frame update
    void Start()
    {
        dog = GameObject.Find("GermanShepherd_Prefab");
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        follow_left = GameObject.Find("follow_left");
        follow_right = GameObject.Find("follow_right");

        //targets
        aim_hand_left = GameObject.Find("AimtargetHandLeft");
        aim_hand_right = GameObject.Find("AimtargetHandRight");
        aim_head = GameObject.Find("AimTargetHead");

        //target obj
        hand_left = GameObject.Find("OVRHandPrefabLeft");
        hand_right = GameObject.Find("OVRHandPrefabRight");
        Player_eyes = GameObject.Find("CenterEyeAnchor");

        //hand positions
        hand_left_position = hand_left.transform.position;
        hand_right_position = hand_right.transform.position;
    }

    public bool hands_moving = false;
    public bool left_hand_tracked = false;
    public bool right_hand_tracked = false;
    public bool left_hand_moving = false;
    public bool right_hand_moving = false;
    // Update is called once per frame
    void Update()
    {
        left_hand_tracked = IsLeftHandTracked();
        right_hand_tracked = IsRightHandTracked();
        if (left_hand_tracked)
        {
            follow_left.transform.position = Vector3.MoveTowards(follow_left.transform.position, hand_left.transform.position, 0.3f);
        }
        if (right_hand_tracked)
        {
            follow_right.transform.position = Vector3.MoveTowards(follow_right.transform.position, hand_right.transform.position, 0.3f);
        }
        left_hand_moving = IsLeftHandMoving();
        right_hand_moving = IsRightHandMoving();
        AreHandsMoving();
        last_fastes = WhoIsFastest();
    }


    float right_hand_tracking_timer = 0;
    float left_hand_tracking_timer = 0;
    float tracking_timer_length = 2f;
    bool IsRightHandTracked()
    {
        right_hand_tracking_timer -= Time.deltaTime;
        Vector3 aim_right_hand_local_pos = aim_head.transform.InverseTransformPoint(hand_right.transform.position);

        if (Vector3.Distance(new Vector3(0.0f, 0.0f, 0.0f), aim_right_hand_local_pos) < 0.18)
        {
            right_hand_tracking_timer = tracking_timer_length;
            return false;
        }
        if (right_hand_tracking_timer < 0)
        {
            return true;
        }
        return false;
    }

    bool IsLeftHandTracked()
    {
        left_hand_tracking_timer -= Time.deltaTime;
        Vector3 aim_left_hand_local_pos = aim_head.transform.InverseTransformPoint(hand_left.transform.position);

        if (Vector3.Distance(new Vector3(0.0f, 0.0f, 0.0f), aim_left_hand_local_pos) < 0.18)
        {
            left_hand_tracking_timer = tracking_timer_length;
            return false;
        }
        if (left_hand_tracking_timer < 0)
        {
            return true;
        }
        return false;
    }


    public float IsCloseToLeftHand()
    {
        basic_behav.dist_left_hand_to_dog = Vector3.Distance(hand_left.transform.position, dog.transform.position);
        return basic_behav.dist_left_hand_to_dog;
    }
    public float IsCloseToRightHand()
    {
        basic_behav.dist_right_hand_to_dog = Vector3.Distance(hand_right.transform.position, dog.transform.position);
        return basic_behav.dist_right_hand_to_dog;
    }

    public bool LeftHandCloser()
    {
        if (IsCloseToLeftHand() < IsCloseToRightHand())
        {
            return true;
        }
        else { return false; }
    }

    float hands_moving_timer = 0f;
    public bool AreHandsMoving()
    {
        return right_hand_moving || left_hand_moving;
    }

    float moving_value = 0.005f;
    float right_hand_moving_timer = 0f;
    float left_hand_moving_timer = 0f;
    float moving_timer_length = 1.5f;
    [SerializeField] float velocity_right;
    [SerializeField] float velocity_left;

    [SerializeField] float velocitimer = 0f;
    [SerializeField] float velo_timer_length = 2f;
    [SerializeField] int last_fastes = 0;

    public int GetFastes()
    {
        return last_fastes;
    }

    public int WhoIsFastest()
    {
        velocitimer -= Time.deltaTime;

        //timerdecay:
        float dv = 0;
        if (last_fastes == 1)
        {
            dv = velocity_left;
        }
        else if (last_fastes == -1)
        {
            dv = velocity_right;
        }
        velocitimer -= dv;

        if (velocitimer > 0)
        {
            return last_fastes;
        }
        else if (velocity_right > velocity_left)
        {
            velocitimer = velo_timer_length;
            return 1; // rechts
        }
        else if (velocity_right < velocity_left)
        {
            velocitimer = velo_timer_length;
            return -1; // links
        }
        return 0;
    }

    public bool IsRightHandMoving()
    {
        right_hand_moving_timer -= Time.deltaTime;


        float distance = Vector3.Distance(hand_right.transform.position, hand_right_position);
        hand_right_position = hand_right.transform.position;

        //SPEED BERECHNUNG:
        //v = d/t
        velocity_right = distance / Time.deltaTime;

        if (!right_hand_tracked)
        {
            return false;
        }
        if (right_hand_moving_timer > 0)
        {
            return true;
        }


        if (distance > moving_value)
        {
            right_hand_moving_timer = moving_timer_length;
            Debug.Log("Right MOVING");
            return true;
        }
        else return false;
    }

    public bool IsLeftHandMoving()
    {
        float distance = Vector3.Distance(hand_left.transform.position, hand_left_position);
        hand_left_position = hand_left.transform.position;

        //SPEED BERECHNUNG:
        //v = d/t
        velocity_left = distance / Time.deltaTime;

        left_hand_moving_timer -= Time.deltaTime;
        if (!left_hand_tracked)
        {
            return false;
        }
        if (left_hand_moving_timer > 0)
        {
            return true;
        }

        if (distance > moving_value)
        {
            left_hand_moving_timer = moving_timer_length;
            Debug.Log("Left MOVING");
            return true;
        }
        else return false;
    }

}
