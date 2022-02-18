using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turning_Direction_Handler : MonoBehaviour
{
    //scripts
    Collision_Detection col_det_left;
    Collision_Detection col_det_right;
    public Collision_Detection col_det_left_trot;
    public Collision_Detection col_det_right_trot;

    Basic_Behaviour basic_behav;
    Animation_Controll anim_controller;
    Animations anim;

    //objects
    GameObject dog;
    GameObject left_cube;
    GameObject right_cube;
    GameObject left_cube_trot;
    GameObject right_cube_trot;

    public bool turn_90_deg;
    public bool turning;


    // Start is called before the first frame update
    void Start()
    {
        //access to other scripts
        dog = GameObject.Find("GermanShepherd_Prefab");
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        anim_controller = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();

        //get obj and scripts - left and right cube
        left_cube = GameObject.Find("left");
        right_cube = GameObject.Find("right");
        left_cube_trot = GameObject.Find("left_trot");
        right_cube_trot= GameObject.Find("right_trot");
        col_det_left = left_cube.GetComponent<Collision_Detection>();
        col_det_right = right_cube.GetComponent<Collision_Detection>();
        col_det_left_trot = left_cube_trot.GetComponent<Collision_Detection>();
        col_det_right_trot = right_cube_trot.GetComponent<Collision_Detection>();
        turning = false;
    }

    bool LeftCollided()
    {
        if (col_det_left.collided || col_det_left_trot.collided)
        {
            return true;
        }
        else return false;
    }
    bool RightCollided()
    {
        if (col_det_right.collided || col_det_right_trot.collided)
        {
            return true;
        }
        else return false;
    }
    bool NoneCollided()
    {
        if (!col_det_right_trot.collided & !col_det_right.collided && !col_det_left_trot.collided && !col_det_left.collided)
        {
            return true;
        }
        else return false;
    }

    // Update is called once per frame
    void Update()
    {
        //if dog is currently not turning
        if (!turning)
        {
            SetTurningDirection();
        }
        //when neither cube is colliding
        if (NoneCollided())
        {
            StopTurning();
        }
        /*if (neutral_behav.dog_state == Basic_Behaviour.Animation_state.walking_after_turning)
        {
            //neutral_behav.change_anim_timer = .3f;
        }*/
    }

    void StopTurning()
    {
        //if we are still turning
        if (turning)
        {
            //stop turning and continue walking
            ToggleTurning();
            SetAnimationState(Basic_Behaviour.Animation_state.walking_after_turning);
            SetAnimationTimerLower(0.75f) ;
        }
    }
    //turn left opr right
    void SetTurningDirection()
    {
        if(basic_behav.dog_state == Basic_Behaviour.Animation_state.standing && (LeftCollided() || RightCollided()))
        {
            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
        }
        //if collision with corner turn 90 degrees left/right
        if (col_det_left.hit_corner || col_det_left_trot.hit_corner)
        {
            //Debug.Log("CORNER left, turn RIGHT");
            turn_90_deg = true;
            SetAnimationState(Basic_Behaviour.Animation_state.turning_right);
            ToggleTurning();
            SetAnimationTimerToZero();
            return;
        }
        if (col_det_right.hit_corner || col_det_right_trot.hit_corner)
        {
            //Debug.Log("CORNER left, turn LEFT");
            turn_90_deg = true;
            SetAnimationState(Basic_Behaviour.Animation_state.turning_left);
            ToggleTurning();
            SetAnimationTimerToZero();
            return;
        }
        //left and right cube collide at the same time --> Turn left and set turning true
        if (RightCollided() && LeftCollided())
        {
            //Debug.Log("BOTH --> Left");
            ToggleTurning();
            SetAnimationState(Basic_Behaviour.Animation_state.turning_left);
            SetAnimationTimerToZero();
            return;
        }
        //left cube collides --> turn right and set turning true
        if (LeftCollided())
        {
            //Debug.Log("COLLIDED left. TURN right");
            SetAnimationState(Basic_Behaviour.Animation_state.turning_right);
            ToggleTurning();
            SetAnimationTimerToZero();
        }
        //right cube collides --> turn left and set turning true
        if (RightCollided())
        {
            //Debug.Log("COLLIDED right. TURN left");
            SetAnimationState(Basic_Behaviour.Animation_state.turning_left);
            ToggleTurning();
            SetAnimationTimerToZero();
        }

    }

    //sets dog_state to an Animation state
    void SetAnimationState(Basic_Behaviour.Animation_state state)
    {
        basic_behav.dog_state = state;
    }

    public void SetAnimationTimerToZero()
    {
        basic_behav.change_anim_timer = 0;
    }

    //enough time to walk away from edges and short enough to not turn infinetly
    public void SetAnimationTimerLower(float time)
    {
        basic_behav.change_anim_timer = time;
    }
    void ToggleTurning()
    {
        turning = !turning;
    }

}
