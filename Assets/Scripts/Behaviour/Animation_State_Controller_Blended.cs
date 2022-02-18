using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_State_Controller_Blended : MonoBehaviour
{
    public Animator animator;
    float X = 0;
    float Y = 0;
    public float increase = 2;
    public float decrease = 2;
    public float max_walking_velocity = 0.5f;
    public float max_trotting_velocity = 2f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool is_walking = Input.GetKey("w");
        bool is_turning_left = Input.GetKey("a");
        bool is_stopping = Input.GetKey("s");
        bool is_turning_right = Input.GetKey("d");
        bool is_running = Input.GetKey("space");

        //sets velocity to running/walking
        float current_max_velocity = is_running ? max_trotting_velocity : max_walking_velocity;
        //walk
        if (is_walking && Y < current_max_velocity && !is_running)
        {
            Y += Time.deltaTime * increase;
        }
        //left
        if (is_turning_left && X > -current_max_velocity & !is_running)
        {
            X -= Time.deltaTime * increase;
        }
        //right
        if (is_turning_right && X < current_max_velocity && !is_running)
        {
            X += Time.deltaTime * increase;
        }
        //break
        if (is_stopping && Y > 0)
        {
            Y -= Time.deltaTime * increase;
        }
        //decrease Y over time
        if (!is_walking && Y > 0)
        {
            Y -= Time.deltaTime * increase;
        }
        //decrease X over time
        if (!is_turning_left && !is_turning_right && X > 0)
        {
            X -= Time.deltaTime * increase;
        }
        //increase X over time
        if (!is_turning_right && !is_turning_left && X < 0)
        {
            X += Time.deltaTime * increase;
        }

        //run
        if (is_running && X > current_max_velocity)
        {
            X = current_max_velocity;
        }
        else if(is_running && X > current_max_velocity)
        {
            X = Time.deltaTime * decrease;
        }

        animator.SetFloat("X", X);
        animator.SetFloat("Y", Y);
    }
}
