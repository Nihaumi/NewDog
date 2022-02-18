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

    MovementUtils MU;
    GameObject agg_position;

    float timer = 1;
    public enum Step
    {
        initial,
        StartTurning,
        TurnToPos,
        WalkToPos,
        LayDown,
        TurnToPlayer,
        FindTarget,
        DistanceFromTarget,
        WaitASecond,
        Stop,
        test
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
        facing_player = false;
        facing_target = false;
        started_walking = false;
        escape_chance = false;
        aggressive = false;
        timer_started = false;

        aggression_animation_counter = 0;
        after_aggression_counter = 0;
        basic_behav.speed = 0.1f;


        MU = dog.GetComponent<MovementUtils>();
        agg_position = GameObject.Find("agg_position");
        current_step = Step.initial;

    }
    // Update is called once per frame
    void Update()
    {

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

    public void MoveToPositionAndFacePlayer()
    {

        switch (current_step)
        {
            case Step.test:
                MU.walk_back();
                break;
            case Step.TurnToPos:
                /* 
                 * 1. drehen
                 * 2. wenn auf target gucken stehen
                 */

                // x_goal = -1

                if (!MU.walk_until_complete_speed(0.75f))
                {
                    MU.start_moving();

                    return;
                }

                MU.reset_acceleration();
                bool are_we_facing_the_agg_target = MU.turn_until_facing(agg_position, true);

                if (are_we_facing_the_agg_target)
                    current_step = Step.WalkToPos;
                break;
            case Step.WalkToPos:
                /*
                 * 3. laufen zum target = pause location
                 */
                bool are_we_touching_the_agg_pos = MU.walk_until_touching(agg_position, 3f, false);

                if (are_we_touching_the_agg_pos)
                    current_step = Step.TurnToPlayer;
                break;
            case Step.TurnToPlayer:
                //drehen Sie sich bitte zum Player um!
                if (!MU.walk_until_complete_speed(0.75f))
                {
                    return;
                }

                bool are_we_facing_the_player = MU.turn_until_facing(player);

                if (are_we_facing_the_player)
                {
                    Debug.Log("FINDING TARGET");
                    current_step = Step.FindTarget;
                }

                break;
            case Step.FindTarget:
                if (MU.looking_directly_at(player))
                {
                    MU.stop_moving();
                    current_step = Step.WaitASecond;
                }
                break;
            case Step.WaitASecond:
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    current_step = Step.DistanceFromTarget;
                }
                break;
            case Step.DistanceFromTarget:
                if (MU.distance_from_target(player))
                {
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
                    current_step = Step.TurnToPos;
                    basic_behav.SetShortTimer(2, 2);
                    break;
                default:
                    break;
            }

        }
        if (current_step == Step.Stop)
        {
            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.after_aggression:
                    dog_audio.StopAllSounds();
                    Debug.Log("AFTER AGGRO");
                    if (after_aggression_counter == 0)
                    {
                        basic_behav.ResetParameter();
                        anim_controll.ChangeAnimationState(anim.aggresive_blend_tree);
                        basic_behav.y_goal = Basic_Behaviour.walking_value;
                        MU.DodgePlayer(player, 4);
                        after_aggression_counter++;
                        basic_behav.SetShortTimer(2, 2);
                    }
                    else if (after_aggression_counter == 1)
                    {
                        basic_behav.TurnLeft();
                        after_aggression_counter++;
                        basic_behav.SetShortTimer(2, 2);

                    }
                    else if (after_aggression_counter == 2)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                        basic_behav.SetShortTimer(2, 2);
                        basic_behav.WalkForward();
                        behav_switch.enter_pause = true;
                    }

                    /*else if (after_aggression_counter == 2)
                    {
                        anim_controll.ChangeAnimationState(anim.trans_stand_to_lying_01);
                        basic_behav.ResetParameter();
                        after_aggression_counter++;
                        basic_behav.SetLongTimer();
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                        dog_audio.PlaySoundAfterPause(dog_audio.panting_calm);
                    }*/
                    
                    break;
                case Basic_Behaviour.Animation_state.aggressiv:
                    aggressive = true;
                    dog_audio.StopAllSounds();
                    if (aggression_animation_counter == 1 % 4 || aggression_animation_counter == 2 % 4)
                    {
                        anim_controll.ChangeAnimationState(anim.aggressive);
                        dog_audio.StopAllSounds();
                        dog_audio.aggressive_bark.Play();
                        aggression_animation_counter++;
                        basic_behav.SetShortTimer(10, 10);
                    }
                    else if (aggression_animation_counter == 0 % 4)
                    {
                        anim_controll.ChangeAnimationState(anim.bite_L);
                        dog_audio.StartCoroutine(dog_audio.PlaySoundAfterAnother(dog_audio.bite_bark, dog_audio.aggressive_bark));
                        aggression_animation_counter++;
                        basic_behav.SetShortTimer(10, 10);
                    }
                    else if (aggression_animation_counter == 5)
                    {
                        anim_controll.ChangeAnimationState(anim.bite_R);
                        dog_audio.StartCoroutine(dog_audio.PlaySoundAfterAnother(dog_audio.bite_bark, dog_audio.aggressive_bark));
                        aggression_animation_counter++;
                        basic_behav.SetShortTimer(10, 10);
                    }
                    else if (aggression_animation_counter == 3 % 4)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.after_aggression;
                        basic_behav.SetShortTimer(2, 2);
                    }

                    ; //TODO set times to 10

                    break;

                case Basic_Behaviour.Animation_state.standing:
                    if (aggressive)
                    {
                        anim_controll.ChangeAnimationState(anim.lying_01);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    }
                    else
                    {
                        dog_audio.StopAllSounds();
                        anim_controll.ChangeAnimationState(anim.stand_agg);
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.aggressiv;
                        basic_behav.SetShortTimer(2, 2);
                    }
                    break;
            }
        }
        Debug.Log("AGRssive FUNCTION");

    }
}
