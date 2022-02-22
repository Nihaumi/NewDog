using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chill_Behaviour : MonoBehaviour
{
    public GameObject dog;
    public GameObject player;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject dog_sound_manager;
    public Animator animator;
    public GameObject player_target;
    public GameObject chill_pos;
    public GameObject chill_pos_2;
    public GameObject chill_pos_3;

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

    public enum Step
    {
        Turning,
        WalkToTarget,
        LookDirectlyAtTarget,
        SwitchToSeek,
        Wait,
        Turning_2,
        WalkToTarget_2,
        Stop,
        initial,
        Onwards,
        dodge,
    }

    [SerializeField] Step current_step;


    public Step GetCurrentStep()
    {
        return current_step;
    }

    float timer;
    float start_timer = 7f;
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
        chill_pos = GameObject.Find("chill_position");
        chill_pos_2 = GameObject.Find("chill_position_2");
        chill_pos_3 = GameObject.Find("chill_position_3");

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

        timer = start_timer;

        current_step = Step.initial;
    }


    // Update is called once per frame
    void Update()
    {

    }


    GameObject target;
    float target_count = 0;
    int num_of_targets = 3;
    float onwards_timer = 0;
    float sniff_timer = 0;
    GameObject SetTarget()
    {
        if (target_count % num_of_targets == 0)
        {
            target = chill_pos;
        }
        else if (target_count % num_of_targets == 2)
        {
            target = chill_pos_2;
        }
        else if (target_count % num_of_targets == 1)
        {
            target = chill_pos_3;
        }
        Debug.Log("CHILL TARGET = " + target);

        return target;
    }
    float speed = 0.85f;
    public void BeChill()
    {
        switch (current_step)
        {
            case Step.dodge:
                Debug.Log("DODGING PLAYER IN ENUM");
                bool doging = MU.DodgePlayer(player_target, 2f);
                if (!doging)
                {
                    current_step = Step.Turning;
                }
                break;
            case Step.Turning:
                /* 
                * 1. drehen
                * 2. wenn auf target gucken stehen
                */
                if (MU.DodgePlayer(player_target, 2f))
                {
                    Debug.Log("YO LISTEN UP HERE IS A STORY!");
                    current_step = Step.dodge;
                    break;
                }
                if (!MU.walk_until_complete_speed(speed))
                {
                    Debug.Log("CHILL TURN");
                    MU.start_moving(1f);

                    return;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_pos = MU.turn_until_facing(SetTarget(), false);

                if (are_we_facing_the_pos || MU.is_touching(SetTarget()))
                {
                    current_step = Step.SwitchToSeek;
                }
                break;
            case Step.SwitchToSeek:
                Debug.Log("CHILL Seek");
                basic_behav.z_acceleration = 0.5f;
                MU.start_seeking();

                sniff_timer += Time.deltaTime;
                if (sniff_timer > 2f)
                {
                    current_step = Step.WalkToTarget;
                }
                break;
            case Step.WalkToTarget:
                if (MU.DodgePlayer(player_target, 2f))
                {
                    Debug.Log("YO LISTEN UP HERE IS A STORY!");
                    current_step = Step.dodge;
                    break;
                }
                bool are_we_touching_the_pos = MU.walk_until_touching(SetTarget(), 0.6f, false);
                basic_behav.y_acceleration = 1f;
                basic_behav.z_acceleration = 1.5f;
                sniff_timer = 0;
                if (are_we_touching_the_pos)
                {
                    basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                    basic_behav.y_goal = Basic_Behaviour.standing_value;
                    Debug.Log("CHILL TOUCH");
                    target_count++;
                    timer = start_timer;
                    animator.SetBool("trans_to_bbt", false);
                    current_step = Step.Wait;
                }

                break;
            case Step.Wait:
                if (basic_behav.y_axis == Basic_Behaviour.standing_value)
                {
                    anim_controll.ChangeAnimationState(anim.stand_02);
                    timer -= Time.deltaTime;
                    animator.SetBool("trans_to_bbt", true);
                }

                if (timer <= 0 && animator.GetCurrentAnimatorStateInfo(0).IsName(anim.bbt))
                {
                    Debug.Log("CHILL WAIT");

                    current_step = Step.Turning;
                }
                break;
            case Step.Stop:

                break;
        }


    }
    public void ChillBehaviour()
    {
        if (current_step == Step.initial)
        {
            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.lying:
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_lying_to_stand);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.standing:
                    current_step = Step.Turning;
                    break;
            }
        }
    }
    float wait_count = 0;
}
