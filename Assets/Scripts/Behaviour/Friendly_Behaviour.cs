using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Friendly_Behaviour : MonoBehaviour
{
    //other script
    public GameObject dog;
    public GameObject player;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject dog_sound_manager;
    public Animator animator;
    public GameObject player_target;

    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Basic_Behaviour basic_behav;
    Neutral_Behaviour neutral_behav;
    PlayerInteraction player_interaction;
    Audio_Sources dog_audio;
    Pause_Behaviour pause_behav;
    MovementUtils MU;
    [SerializeField] Behaviour_Switch behav_switch;
    [SerializeField] GameObject behaviour_manager;

    [SerializeField] float friendly_time;
    public double friendly_goal_dist_to_player = 3f;

    public enum Step
    {
        Turning,
        WalkToTarget,
        LookDirectlyAtTarget,
        SitDown,
        Stop,
        initial
    }

    [SerializeField] Step current_step;

    public Step GetCurrentStep()
    {
        return current_step;
    }

    // Start is called before the first frame update
    void Start()
    {

        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.FindGameObjectWithTag("Player");
        player_target = GameObject.Find("target");
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
        MU = dog.GetComponent<MovementUtils>();

        facing_player = false;
        escape_chance_on = false;
        friendly = false;
        friendly_time = 3f;//TODO anpassen

        after_friendly_anim_counter = 0;
        current_step = Step.initial;
    }



    // Update is called once per frame
    void Update()
    {

    }
    /* Fix Rigs --> broken neck
     */
    public bool started_walking = false;

    /*
    *   false if dog to far away from player
    *   true if dog sits down near the player
    */
    public bool escape_chance_on;

    /*
    *   If we touch the player AND we turned = true
    *   NO FALSE STATE OTHER THAN START
    */
    public bool facing_player;


    /*
    *   On Start: friendly = false
    *   On "first" animation change in FriendlyBehaviour: friendly = true 
    */
    public bool friendly;
    public void ApproachPlayer()
    {
        switch (current_step)
        {
            case Step.Turning:
                /* 
                 * 1. drehen
                 * 2. wenn auf target gucken stehen
                 */
                if (!MU.walk_until_complete_speed(0.85f))
                {
                    MU.start_moving();

                    return;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_player = MU.turn_until_facing(player_target, true);

                if (are_we_facing_the_player || MU.is_touching(player_target))
                    current_step = Step.WalkToTarget;
                break;
            case Step.WalkToTarget:
                /*
                 * 3. laufen zum target
                 */
                bool are_we_touching_the_player = MU.walk_until_touching(player_target, 2f);

                if (are_we_touching_the_player)
                    current_step = Step.SitDown;
                break;
            case Step.SitDown:
                if (MU.walk_until_complete_speed(0.001f))
                {
                    MU.sit_down();
                    basic_behav.change_anim_timer = 30f; //TODO anpassen
                    current_step = Step.Stop;
                }

                break;
            case Step.Stop:
                /*
                 * 4. Do nothing
                 */

                break;
            default:
                break;
        }

    }

    /*
    *   Changes the dog behaviour between standing, sitting, lying, sleeping and walking.
    *       Also changes the x and y goals.
    *       sets friendly
    */
    [SerializeField] int after_friendly_anim_counter;
    public void FriendlyBehaviour()
    {
        if (current_step == Step.initial)
        {//3 seconds to go from one state to another
            Debug.Log("So are you gonna do itHUH");
            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.standing:

                    basic_behav.ResetParameter();
                    dog_audio.StopAllSounds();
                    friendly = true;
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.friendly_walking;
                    current_step = Step.Turning;
                    basic_behav.SetShortTimer(10, 15);

                    break;
                case Basic_Behaviour.Animation_state.pause:
                    basic_behav.ResetParameter();
                    dog_audio.StopAllSounds();
                    friendly = true;
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.sitting:
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_sit_to_stand);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.lying:
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_lying_to_stand);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.sleeping:
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_sleep_to_stand);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.walking:
                    Debug.Log("friends and walking");
                    dog_audio.StopAllSounds();
                    basic_behav.WalkForward();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    if (basic_behav.y_goal == Basic_Behaviour.trot_value)
                    {
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                    }
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                default:
                    break;
            }
        }
        if (current_step == Step.Stop)
        {
            Debug.Log("ENTER FRIENDLY RANDOM BEHAV");
            dog_audio.StopAllSounds();
            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.friendly_walking:

                    Debug.Log("FRIENDLY WALKING");
                    if (after_friendly_anim_counter == 0)
                    {
                        basic_behav.ResetParameter();
                        anim_controll.ChangeAnimationState(anim.bbt_trans_sit_to_stand);
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                        basic_behav.SetShortTimer(3, 3);
                        after_friendly_anim_counter++;
                    }
                    else if (after_friendly_anim_counter == 1)
                    {
                        basic_behav.ResetParameter();
                        //anim_controll.ChangeAnimationState(anim.aggresive_blend_tree);
                        basic_behav.y_goal = Basic_Behaviour.walking_value;
                        basic_behav.choose_direction_to_walk_into(player, true);
                        basic_behav.SetShortTimer(2, 2);
                        after_friendly_anim_counter++;
                    }
                    else if (after_friendly_anim_counter == 2)
                    {
                        basic_behav.WalkForward();
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                        basic_behav.SetShortTimer(1, 1);
                        after_friendly_anim_counter++;
                    }
                    else if (after_friendly_anim_counter == 3)
                    {
                        basic_behav.ResetParameter();
                        anim_controll.ChangeAnimationState(anim.trans_stand_to_lying_00);
                        StartCoroutine(dog_audio.PlaySoundAfterPause(dog_audio.panting_calm));
                        basic_behav.SetShortTimer(10, 10);//TODO Set time right
                        after_friendly_anim_counter++;

                    }
                    else if (after_friendly_anim_counter == 4)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                        behav_switch.enter_pause = true;
                    }
                    /*else if (after_friendly_anim_counter == 3)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_sit_to_turn_walk);
                        basic_behav.y_goal = basic_behav.walking_slow_value;
                        //decide which direction to turn 
                        if (basic_behav.GetPlayerOffset(0, 32, 0.12f, false) == -1)
                        {
                            basic_behav.TurnLeft();
                        }
                        else basic_behav.TurnRight();
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                        basic_behav.SetShortTimer(3, 3);
                        after_friendly_anim_counter++;
                    }*/
                    break;
                case Basic_Behaviour.Animation_state.standing:

                    basic_behav.ResetParameter();
                    dog_audio.StopAllSounds();
                    //Debug.Log("standinglist item at rndindex: " + basic_behav.random_index + "is:" + anim.list_standing[basic_behav.random_index]);
                    if (basic_behav.random_index == 0)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_list_standing[basic_behav.random_index]);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                        //audio
                        dog_audio.PlaySoundAfterPause(dog_audio.panting_calm);
                    }
                    if (basic_behav.random_index == 1)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_list_standing[basic_behav.random_index]);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.sitting;
                        //audio
                        dog_audio.PlaySoundAfterPause(dog_audio.panting_calm);
                    }
                    if (basic_behav.random_index > 1 && basic_behav.random_index < 4)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt);
                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        //SetBlendTreeParameters();
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetShortTimer(7, 10);
                    Debug.Log("standing list item at rndindex: " + basic_behav.random_index + "is:" + anim.friendly_list_standing[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.sitting:
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    dog_audio.StopAllSounds();
                    if (basic_behav.random_index == 0)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt_trans_sit_to_stand);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    }
                    basic_behav.ResetParameter();
                    if (basic_behav.random_index == 1)
                    {
                        basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                    }
                    if (basic_behav.random_index == 2)
                    {
                        basic_behav.y_goal = Basic_Behaviour.walking_value;
                    }

                    basic_behav.SetShortTimer(7, 15);
                    Debug.Log("sitting list item at rndindex: " + basic_behav.random_index + "is:" + anim.friendly_list_sitting[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.lying:
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();

                    if (basic_behav.random_index == 0)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_list_lying[basic_behav.random_index]);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.sleeping;
                    }
                    else
                    {
                        anim_controll.ChangeAnimationState(anim.bbt_trans_sit_to_stand);

                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetShortTimer(7, 10);
                    Debug.Log("lyingg list item at rndindex: " + basic_behav.random_index + "is:" + anim.friendly_list_lying[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.sleeping:

                    basic_behav.ResetParameter();
                    dog_audio.StopAllSounds();
                    //Debug.Log("sleeping list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_sleeping[basic_behav.random_index]);
                    if (basic_behav.random_index == 0)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_list_sleeping[basic_behav.random_index]);

                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                        //audio                  
                        dog_audio.PlaySoundAfterPause(dog_audio.panting_calm);
                    }
                    else if (basic_behav.random_index == 1)
                    {
                        anim_controll.ChangeAnimationState(anim.friendly_list_sleeping[basic_behav.random_index]);

                        basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    }
                    else
                    {
                        anim_controll.ChangeAnimationState(anim.bbt_trans_sleep_to_stand);
                        if (basic_behav.random_index == 2)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 3)
                        {
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                    }
                    basic_behav.SetLongTimer();
                    Debug.Log("sleeping list item at rndindex: " + basic_behav.random_index + "is:" + anim.friendly_list_sleeping[basic_behav.random_index]);
                    break;

                case Basic_Behaviour.Animation_state.walking:
                    dog_audio.StopAllSounds();

                    if (anim_controll.current_state != anim.bbt)
                    {
                        anim_controll.ChangeAnimationState(anim.bbt);
                        basic_behav.WalkForward();
                    }
                    basic_behav.SetLongTimer();
                    if (basic_behav.random_index == 0)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                    }
                    else
                    {
                        if (basic_behav.random_index == 1)
                        {

                            basic_behav.y_goal = Basic_Behaviour.walking_slow_value;
                        }
                        if (basic_behav.random_index == 2)
                        {

                            basic_behav.y_goal = Basic_Behaviour.walking_value;
                        }

                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;

                    }
                    Debug.Log("walking list item at rndindex: " + basic_behav.random_index + "is:" + anim.friendly_list_walking[basic_behav.random_index]);
                    break;
                default:
                    return;
            }
        }
    }


}
