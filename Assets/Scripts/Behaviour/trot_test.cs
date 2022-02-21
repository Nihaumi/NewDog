using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trot_test : MonoBehaviour
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
    }

    [SerializeField] Step current_step;
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

        current_step = Step.Turn;
    }

    // Update is called once per frame
    void Update()
    {
        DoTrot();
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

    int dodge_count = 0;
    int target_count = 1;
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
                
                if (dodge_timer > 1f && dodge_count <1)
                {
                    dodge_count++;
                    dodge_timer = 0;
                    current_step = Step.Turn;
                }
                else if(dodge_count >0)
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
                bool are_we_facing_the_pos = MU.turn_until_facing(chill_pos_3, false); ;

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
                    if(dodge_count < 1)
                    {
                        Debug.Log("YO LISTEN UP HERE IS A STORY!");
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_no_standing_value);
                        basic_behav.z_acceleration = 60f;
                        basic_behav.x_goal = -Basic_Behaviour.trot_value;
                        basic_behav.x_axis = -Basic_Behaviour.trot_value;
                        basic_behav.y_goal = Basic_Behaviour.standing_value;
                        basic_behav.y_acceleration = 4f;
                        basic_behav.x_acceleration = 4f;
                    }
                    current_step = Step.dodge;
                    return;
                }
                else
                {
                    bool are_we_touching_the_pos = MU.walk_until_touching(chill_pos_3, 1f, false);
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
                            //target_count++;
                            current_step = Step.Turn;
                        }
                        else if (target_count == 1)
                        {
                            basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                            basic_behav.change_anim_timer = 3f;//TODO anpassen
                            behav_switch.SetNeutralTimer(30);
                            basic_behav.y_goal = Basic_Behaviour.walking_value;
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
            case Step.Stop:
                target_count = 0;
                break;
        }
    }
}
