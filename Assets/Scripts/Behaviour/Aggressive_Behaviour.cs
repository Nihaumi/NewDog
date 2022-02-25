using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggressive_Behaviour : MonoBehaviour
{
    //other script
    public GameObject dog;
    public GameObject player;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject dog_sound_manager;
    public Animator animator;
    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Basic_Behaviour basic_behav;
    Neutral_Behaviour neutral_behav;
    PlayerInteraction player_interaction;
    Audio_Sources dog_audio;
    Pause_Behaviour pause_behav;

    Behaviour_Switch behav_switch;
    GameObject behaviour_manager;


    GameObject borders;

    MU_aggro MU;
    GameObject agg_position;

    float y_goal;
    float x_goal;

    GameObject circle_stopper;

    int slow_down_counter = 0;
    [SerializeField] int aggro_counter = 0;
    [SerializeField] float anim_timer = 0;

    [SerializeField] float dist;
    float start_jump_dist;
    float speed;

    float timer = 1;
    public enum Step
    {
        initial,
        turn_to_position,
        go_to_position,
        slow_down,
        turn_to_player,
        go_to_player,
        jump_anim,
        timer,
        trot_away,
        trot_turn_to_player,
        Stop,
        final_jump,
        chill,
        circle_player,
    }
    [SerializeField] Step current_step;

    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.Find("target");
        //player = GameObject.FindGameObjectWithTag("Player");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
        behaviour_manager = GameObject.Find("Behaviour_Manager");
        behav_switch = behaviour_manager.GetComponent<Behaviour_Switch>();
        animator = dog.GetComponent<Animator>();
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        turn_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        neutral_behav = dog.GetComponent<Neutral_Behaviour>();
        player_interaction = player.GetComponent<PlayerInteraction>();
        dog_audio = dog_sound_manager.GetComponent<Audio_Sources>();
        pause_behav = dog.GetComponent<Pause_Behaviour>();


        basic_behav.turning_in_place = false;
        basic_behav.speed = 0.1f;


        agg_position = GameObject.Find("agg_position");
        current_step = Step.initial;

        MU = dog.GetComponent<MU_aggro>();
        agg_position = GameObject.Find("agg_position");

        borders = GameObject.Find("Borders");
        circle_stopper = GameObject.Find("circle_stopper");

        y_goal = basic_behav.y_goal;
        x_goal = basic_behav.x_goal;

        current_step = Step.initial;
        borders.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        GetDistance();
    }
    void GetDistance()
    {
        dist = Vector3.Distance(player.transform.position, dog.transform.position);
    }

    //on start false
    bool is_aggressive = false;
    public bool facing_player;
    public bool facing_target; // in logic handler true
    public bool started_walking; // in logic Handler true
    public bool escape_chance;
    bool aggressive;    //in animation state aggressive true
    bool timer_started; //false, true wehn dog at target and standing

    public float dist_to_target;
    public float reached_target = 1f;

    /*turn in direction of green cube ( aggression position) and move there while walking animation plays
     * if distance between dig and cube is smaller than 1, dog reached cube
     * rotate towards the player and play standing animation 
     * activate agressive animiation state and play aggressive animation
     */
    /*TODO
     * animations are snappy
     * FIX WEIRD TURNING BEHAVIOUR
     * make head rigging look in camera
     * work on if position not yet reached part 
     */
    public bool turn_to_player = false;

    public void BeAggressive()
    {
        switch (current_step)
        {
            case Step.turn_to_position:

                y_goal = Basic_Behaviour.trot_value;
                x_goal = y_goal;
                if (MU.turn_until_facing(agg_position, 1.5f, true))
                {
                    y_goal = Basic_Behaviour.trot_value;
                    current_step = Step.go_to_position;
                }
                y_goal = Basic_Behaviour.trot_value;
                x_goal = y_goal;
                break;

            case Step.go_to_position:
                if (MU.walk_until_touching(agg_position, 1f, false, 2f))
                {
                    current_step = Step.slow_down;
                }
                break;

            case Step.slow_down:
                basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_no_standing_value);
                basic_behav.track_head_in_aggro_mode = false;
                if (slow_down_counter % 2 == 1)
                {

                    if (MU.slow_down(1))
                    {

                        current_step = Step.jump_anim;
                    }

                }
                else
                {
                    if (MU.slow_down(1))
                    {
                        current_step = Step.turn_to_player;
                    }

                }

                break;

            case Step.turn_to_player:
                basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_no_standing_value);
                if (MU.turn_until_facing(player, 1f, true, false))
                {
                    current_step = Step.go_to_player;
                }
                break;

            case Step.go_to_player:

                basic_behav.set_bbt_values(false, 1.5f);
                //basic_behav.y_acceleration = 6f;
                if (aggro_counter >= 2)
                {
                    start_jump_dist = 7f;

                }
                else
                {
                    start_jump_dist = 5f;
                }
                if (MU.walk_until_touching(player, start_jump_dist, false, 2f))
                {
                    slow_down_counter++;
                    current_step = Step.jump_anim;
                }

                break;

            case Step.jump_anim:
                basic_behav.track_head_in_aggro_mode = true;
                basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                anim_controll.ChangeAnimationState(anim.bite_L);
                if (aggro_counter >= 2)
                {
                    anim_timer = 5f;

                }
                else
                {
                    anim_timer = 10f;
                }
                aggro_counter++;
                current_step = Step.timer;

                break;

            case Step.timer:
                anim_timer -= Time.deltaTime;
                if (TimerIsDone(anim_timer))
                {
                    if (aggro_counter >= 3)
                    {
                        current_step = Step.final_jump;
                    }
                    else
                    {
                        current_step = Step.trot_away;
                    }
                }
                break;

            case Step.trot_away:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName(anim.bbt))
                {
                    Debug.Log("NOW NOW NOW!!!!!!!!!!");
                    MU.change_blend_tree_if_necessary(false, true);
                    y_goal = 2f;

                    if (!MU.is_touching(player, 7.5f))
                    {
                        slow_down_counter++;
                        basic_behav.TurnRight();
                        current_step = Step.trot_turn_to_player;
                    }
                }
                else
                {
                    basic_behav.track_head_in_aggro_mode = false;
                    anim_controll.ChangeAnimationState(anim.agg_trot_turn_L_short);
                }

                break;

            /*case Step.circle_player:

                if (MU.is_touching(circle_stopper, 3f))
                {
                    current_step = Step.trot_turn_to_player;
                }

                break;*/

            case Step.trot_turn_to_player:

                if (MU.turn_until_facing(player, 2f, true, true))
                {
                    MU.change_blend_tree_if_necessary(false, true);
                    y_goal = 2f;
                    current_step = Step.go_to_player;
                }

                break;

            case Step.final_jump:
                basic_behav.ResetParameter();
                basic_behav.track_head_in_aggro_mode = true;
                basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                anim_controll.ChangeAnimationState(anim.bite_R);
                aggro_counter = 0;
                if (dist < 1f)
                {
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    basic_behav.y_goal = 1f;
                    basic_behav.choose_direction_to_walk_into(player, true);
                    current_step = Step.chill;
                }

                break;

            case Step.chill:
                basic_behav.track_head_in_aggro_mode = false;
                Debug.Log("CHILLL");
                if (!MU.is_touching(player, 1f))
                {
                    MU.change_blend_tree_if_necessary(false, true);
                    x_goal = 2f;
                    basic_behav.WalkForward();
                    y_goal = 2f;
                    current_step = Step.Stop;

                }

                break;

            case Step.Stop:
                Debug.Log("aggro test done");
                break;
        }
    }
    bool TimerIsDone(float timer)
    {
        if (timer <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float dist_to_player;
    public float close_enough_to_player = 2f;
    public bool aggressive_too_close = false;
    public void StopAgression()
    {
        if (facing_player)
        {
            if (dist_to_player < close_enough_to_player || aggression_animation_counter > 3)
            {
                Debug.Log("STOP");
                dog_audio.StopAllSounds();
                anim_controll.ChangeAnimationState(anim.trans_agg_to_stand);
                basic_behav.dog_state = Basic_Behaviour.Animation_state.after_aggression;
                basic_behav.change_anim_timer = 4;
                facing_player = false;
                //tell behav switch to switch to neutral/friendly
                aggressive_too_close = true;
            }
        }
    }

    //agression animation manager
    [SerializeField] int aggression_animation_counter;
    [SerializeField] int after_aggression_counter;
    public void AggressiveBehaviour()
    {
        if (current_step == Step.initial)
        {
            basic_behav.z_goal = Basic_Behaviour.bbt_standing_value;
            switch (basic_behav.dog_state)
            {
                //after agressive: stand
                //then turn away to the left
                //lay down

                case Basic_Behaviour.Animation_state.lying:
                    if (aggressive)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    }
                    else
                    {
                        dog_audio.StopAllSounds();
                        anim_controll.ChangeAnimationState(anim.agg_trans_lying_to_stand);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                        basic_behav.SetShortTimer(2, 2);
                    }
                    break;
                case Basic_Behaviour.Animation_state.sleeping:
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.agg_trans_sleep_to_stand);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                case Basic_Behaviour.Animation_state.pause:
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                case Basic_Behaviour.Animation_state.sitting:
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.agg_trans_sit_to_stand);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                case Basic_Behaviour.Animation_state.walking:
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.y_goal = Basic_Behaviour.standing_value;
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                case Basic_Behaviour.Animation_state.standing:
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.ResetParameter();
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.aggressiv;
                    current_step = Step.turn_to_position;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                default:
                    break;
            }

        }
        if (current_step == Step.Stop)
        {
            basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
            borders.SetActive(true);
            behav_switch.enter_pause = true;

        }


    }
}
