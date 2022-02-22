using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral_Behaviour : MonoBehaviour
{
    //other script
    public GameObject dog;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public Animator animator;
    public GameObject dog_sound_manager;
    GameObject behav_manager;
    public GameObject player;

    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Basic_Behaviour basic_behav;
    Behaviour_Switch behav_switch;
    Audio_Sources dog_audio;
    Pause_Behaviour pause_behav;
    MovementUtils MU;

    GameObject chill_pos;
    GameObject chill_pos_2;
    GameObject chill_pos_3;
    GameObject agg_pos;
    public enum Step
    {
        Turn,
        WalkToTarget,
        Trot,
        Stop,
        initial,
        dodge,
        AfterDodge
    }

    public Step current_step;
    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.FindGameObjectWithTag("Player");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
        behav_manager = GameObject.Find("Behaviour_Manager");

        animator = dog.GetComponent<Animator>();
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        turn_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        behav_switch = behav_manager.GetComponent<Behaviour_Switch>();
        dog_audio = dog_sound_manager.GetComponent<Audio_Sources>();
        pause_behav = dog.GetComponent<Pause_Behaviour>();
        MU = dog.GetComponent<MovementUtils>();

        chill_pos = GameObject.Find("chill_position");
        chill_pos_2 = GameObject.Find("chill_position_2");
        chill_pos_3 = GameObject.Find("chill_position_3");
        agg_pos = GameObject.Find("agg_position_1");

        current_step = Step.initial;
    }

    // Update is called once per frame
    void Update()
    {
        if ((behav_switch.GetNeutralTimer() <= behav_switch.GetNeutralTime() / 2) && current_step == Step.initial)
        {
            current_step = Step.Turn;
        }
    }
    GameObject SetTarget()
    {
        if (target_count % num_of_targets == 1)
        {
            target = chill_pos_3;
        }
        else if (target_count % num_of_targets == 0)
        {
            target = agg_pos;
        }
        Debug.Log("NEUTRAL TARGET = " + target);

        return target;
    }
    [SerializeField] int dodge_count = 0;
    int target_count = 0;
    GameObject target;
    float num_of_targets = 2;
    [SerializeField] float dodge_timer = 0;
    public void DoTrot()
    {
        switch (current_step)
        {
            case Step.dodge:
                Debug.Log("DODGING PLAYER IN ENUM");

                dodge_timer += Time.deltaTime;

                if (dodge_timer > 1f && dodge_count < 1)
                {
                    dodge_count++;
                    dodge_timer = 0;
                    current_step = Step.AfterDodge;
                }
                else if (dodge_count > 0)
                {
                    bool doging = MU.DodgePlayer(player, 2f);
                    if (!doging)
                    {
                        current_step = Step.Turn;
                    }
                }
                break;
            case Step.Turn:
                MU.reset_acceleration();
                basic_behav.z_acceleration = 6f;
                behav_switch.SetNeutralTimer(60);
                if (!MU.walk_until_complete_speed(0.85f))
                {
                    Debug.Log("NEUTRAL TURN");
                    MU.start_moving(1f);

                    return;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_pos = MU.turn_until_facing(SetTarget(), false); ;

                if (are_we_facing_the_pos || MU.is_touching(SetTarget()))
                {
                    if (target_count == 0)
                    {
                        current_step = Step.WalkToTarget;
                    }
                    else
                    {
                        current_step = Step.Trot;
                    }
                }

                break;
            case Step.WalkToTarget:
                if (MU.is_touching(player, 3f))
                {
                    if (dodge_count == 0)
                    {
                        Debug.Log("YO LISTEN UP HERE IS A STORY!");
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_no_standing_value);
                        //basic_behav.z_acceleration = 60f;
                        basic_behav.x_goal = -Basic_Behaviour.trot_value;
                        basic_behav.x_axis = -Basic_Behaviour.trot_value;
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                        basic_behav.y_acceleration = 4f;
                        basic_behav.x_acceleration = 4f;
                    }
                    else
                    {
                        basic_behav.y_goal = Basic_Behaviour.walking_value;
                    }
                    current_step = Step.dodge;
                    return;
                }
                else
                {
                    bool are_we_touching_the_pos = MU.walk_until_touching(SetTarget(), 1f, false);
                    if (target_count == 1)
                    {
                        basic_behav.y_goal = Basic_Behaviour.trot_value;
                    }
                    if (are_we_touching_the_pos)
                    {
                        basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                        Debug.Log("NEUTRAL TOUCH");
                        animator.SetBool("trans_to_bbt", false);
                        if (target_count == 0)
                        {
                            target_count++;
                            current_step = Step.Turn;
                        }
                        else if (target_count == 1)
                        {
                            basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                            basic_behav.change_anim_timer = 1f;//TODO anpassen
                            behav_switch.SetNeutralTimer(30);
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                            basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                            current_step = Step.Stop;
                        }

                    }
                }

                break;
            case Step.Trot:
                behav_switch.SetNeutralTimer(60);
                basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                basic_behav.WalkForward();
                basic_behav.y_goal = Basic_Behaviour.trot_value;
                basic_behav.y_acceleration = 2f;
                current_step = Step.WalkToTarget;
                break;

            case Step.AfterDodge:
                basic_behav.WalkForward();
                basic_behav.x_acceleration = 1f;
                basic_behav.y_goal = basic_behav.default_y_acceleration;
                basic_behav.change_anim_timer = 5f;//TODO anpassen
                behav_switch.SetNeutralTimer(15);
                //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                if (basic_behav.y_axis == Basic_Behaviour.walking_slow_value)
                    current_step = Step.Stop;
                break;

            case Step.Stop:
                MU.reset_acceleration();
                target_count = 0;
                break;
        }
    }

    /* transitions von sit/lay/stand in den seek tree! mit trigger 
    * makes it smoother
    */
    public void SetBoolForSeekBT()
    {
        animator.SetBool("go_seek", true);
    }
    public void SetBoolForWalkingBT()
    {
        animator.SetBool("go_seek", false);
    }

    public void NeutralBehaviour()
    {
        if (current_step == Step.initial || current_step == Step.Stop)
        {


            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.friendly_walking:
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    basic_behav.WalkForward();
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    break;

                case Basic_Behaviour.Animation_state.standing:
                    //audio
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    SetBoolForWalkingBT();
                    //Debug.Log("standinglist item at rndindex: " + basic_behav.random_index + "is:" + anim.list_standing[basic_behav.random_index]);
                    if (basic_behav.random_index == 0)
                    {
                        //anim_controll.ChangeAnimationState(anim.list_standing[basic_behav.random_index]);
                        //basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    }
                    else if (basic_behav.random_index == 1)
                    {
                        //anim_controll.ChangeAnimationState(anim.list_standing[basic_behav.random_index]);
                        //basic_behav.dog_state = Basic_Behaviour.Animation_state.sitting;
                        basic_behav.random_index = 5;
                    }
                    if (basic_behav.random_index > 1 && basic_behav.random_index < 6)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt);

                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        if (basic_behav.random_index == 4)
                        {
                            anim_controll.ChangeAnimationState(anim.bbt);
                            basic_behav.set_bbt_values(true, Basic_Behaviour.bbt_standing_value);
                            basic_behav.y_goal = Basic_Behaviour.seek_value;

                        }
                        if (basic_behav.random_index == 5)
                        {
                            //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            //basic_behav.y_goal = Basic_Behaviour.trot_value;
                        }
                        //SetBlendTreeParameters();
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetShortTimer(5, 5);
                    Debug.Log("standing list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_standing[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.sitting:
                    //audio
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_sit_to_stand);

                    SetBoolForWalkingBT();

                    if (basic_behav.random_index == 0)
                    {
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                    }
                    if (basic_behav.random_index == 1)
                    {
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        basic_behav.y_goal = Basic_Behaviour.walking_value;
                    }
                    if (basic_behav.random_index == 2)
                    {
                        SetBoolForSeekBT();
                        basic_behav.set_bbt_values(true, Basic_Behaviour.bbt_standing_value);
                        basic_behav.y_goal = Basic_Behaviour.seek_value;
                    }
                    if (basic_behav.random_index == 3)
                    {
                        //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        //basic_behav.y_goal = Basic_Behaviour.trot_value;
                    }
                    //SetBlendTreeParameters();
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;

                    basic_behav.SetLongTimer();
                    Debug.Log("sitting list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_sitting[basic_behav.random_index]);
                    break;
                case Basic_Behaviour.Animation_state.lying:
                    //audio
                    dog_audio.StopAllSounds();

                    basic_behav.ResetParameter();
                    //Debug.Log("lying list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_lying[basic_behav.random_index]);
                    if (basic_behav.random_index == 0)
                    {
                        //anim_controll.ChangeAnimationState(anim.list_lying[basic_behav.random_index]);
                        //basic_behav.dog_state = Basic_Behaviour.Animation_state.sleeping;
                    }
                    if (basic_behav.random_index > 0)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt_trans_lying_to_stand);
                        SetBoolForWalkingBT();

                        if (basic_behav.random_index == 1)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            SetBoolForSeekBT();
                            basic_behav.set_bbt_values(true, Basic_Behaviour.bbt_standing_value);
                            basic_behav.y_goal = Basic_Behaviour.seek_value;
                        }
                        if (basic_behav.random_index == 4)
                        {
                            //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            //basic_behav.y_goal = Basic_Behaviour.trot_value;
                        }
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetLongTimer();
                    Debug.Log("lyingg list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_lying[basic_behav.random_index]);
                    break;
                case Basic_Behaviour.Animation_state.sleeping:

                    basic_behav.ResetParameter();
                    SetBoolForWalkingBT();
                    //Debug.Log("sleeping list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_sleeping[basic_behav.random_index]);
                    if (basic_behav.random_index == 0)
                    {
                        //anim_controll.ChangeAnimationState(anim.list_sleeping[basic_behav.random_index]);

                        //basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;

                    }
                    else if (basic_behav.random_index == 1)
                    {
                        //anim_controll.ChangeAnimationState(anim.list_sleeping[basic_behav.random_index]);

                        basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    }
                    else
                    {
                        anim_controll.ChangeAnimationState(anim.bbt_trans_sleep_to_stand);

                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        if (basic_behav.random_index == 4)
                        {
                            SetBoolForSeekBT();
                            basic_behav.set_bbt_values(true, Basic_Behaviour.bbt_standing_value);
                            basic_behav.y_goal = Basic_Behaviour.seek_value;
                        }
                        if (basic_behav.random_index == 5)
                        {
                            //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            //basic_behav.y_goal = Basic_Behaviour.trot_value;
                        }
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetLongTimer();
                    Debug.Log("sleeping list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_sleeping[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.walking:
                    //audio
                    dog_audio.StopAllSounds();
                    basic_behav.WalkForward();
                    if (anim_controll.current_state != anim.bbt)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt);
                    }

                    basic_behav.SetLongTimer();

                    if (basic_behav.random_index == 0)
                    {
                        // basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                        //basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        //basic_behav.y_goal = Basic_Behaviour.standing_value;
                    }
                    else
                    {
                        //anim_controll.ChangeAnimationState(anim.blend_tree);
                        if (basic_behav.random_index == 1)
                        {
                            /*if (anim_controll.current_state == anim.blend_tree_seek)
                            {
                                SwitchToOrFromSeekingBehaviour(anim.blend_tree);
                            }*/
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.set_bbt_values(true, Basic_Behaviour.bbt_standing_value);
                            basic_behav.y_goal = Basic_Behaviour.seek_value;
                        }
                        if (basic_behav.random_index == 4)
                        {
                            //audio
                            basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                            /*if (turn_dir_handler.col_det_left_trot.trot_collider_touching_wall || turn_dir_handler.col_det_right_trot.trot_collider_touching_wall)
                            {
                                Debug.Log("wanted to trot but was colliding");
                                break;
                            }
                            else if (!turn_dir_handler.col_det_left_trot.trot_collider_touching_wall && !turn_dir_handler.col_det_right_trot.trot_collider_touching_wall)
                            {
                                if (basic_behav.y_goal == Basic_Behaviour.seek_value)
                                {
                                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                                }
                                basic_behav.y_goal = Basic_Behaviour.trot_value;
                                basic_behav.SetShortTimer(0.1f, 1f);
                            }*/

                        }

                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;

                    }
                    Debug.Log("walking list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_walking[basic_behav.random_index]);
                    break;
                default:
                    return;
            }
        }
    }


    /*
 * when seeking: current anim state = seeking BT
 * from seeking to other walking state:
 * decrease y value in seeking BT until 0
 * change BT to bland tree 
 * y value = chosen value
 * 
*/

    /*TODO:
     * 
     * adjust timing for trotting
     */


}
