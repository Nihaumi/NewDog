using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTest : MonoBehaviour
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
    Basic_Behaviour basic_behav;
    PlayerInteraction player_interaction;

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
        walk_back,
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
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        player_interaction = player.GetComponent<PlayerInteraction>();
        MU = dog.GetComponent<MU_aggro>();
        agg_position = GameObject.Find("agg_position");

        borders = GameObject.Find("Borders");
        circle_stopper = GameObject.Find("circle_stopper");

        y_goal = basic_behav.y_goal;
        x_goal = basic_behav.x_goal;

        current_step = Step.turn_to_position;
        borders.SetActive(false);

        track_head = false;
    }
    public bool track_head;
    // Update is called once per frame
    void Update()
    {
        GetDistance();
        BeAggressive();
    }

    void GetDistance()
    {
        dist = Vector3.Distance(player.transform.position, dog.transform.position);
    }

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
                if (MU.walk_until_touching(agg_position, 2f, false, 2f))
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

                if (MU.turn_until_facing(player, 1f, true, true, false, 20f))
                {
                    current_step = Step.go_to_player;
                }


                break;

            case Step.go_to_player:

                basic_behav.set_bbt_values(false, 2f);
                if (aggro_counter >= 2)
                {
                    start_jump_dist = 4.5f;//

                }
                else
                {
                    start_jump_dist = 4.5f;
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
                        current_step = Step.walk_back;
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

            case Step.trot_turn_to_player:

                if (MU.turn_until_facing(player, 2f, true, false, true))
                {
                    MU.change_blend_tree_if_necessary(false, true);
                    y_goal = 2f;
                    current_step = Step.go_to_player;
                }

                break;

            case Step.walk_back:

                if (MU.is_touching(player, 4f))
                {
                    animator.SetBool("walk_back", true);
                }
                else
                {
                    current_step = Step.final_jump;
                }

                break;

            case Step.final_jump:
                basic_behav.ResetParameter();
                basic_behav.track_head_in_aggro_mode = true;
                basic_behav.y_acceleration = basic_behav.default_y_acceleration;
                anim_controll.ChangeAnimationState(anim.bite_R);
                aggro_counter = 0;
                if (dist < 1.3f)
                {
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.y_goal = 1f;
                    basic_behav.choose_direction_to_walk_into(player, true);
                    current_step = Step.chill;
                }

                break;

            case Step.chill:
                basic_behav.track_head_in_aggro_mode = false;
                Debug.Log("CHILLL");
                if (!MU.is_touching(player, 1.2f))
                {
                    MU.change_blend_tree_if_necessary(false, true);
                    x_goal = 2f;
                    basic_behav.WalkForward();
                    y_goal = 2f;
                    animator.SetBool("walk_back", false);
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
    //step 0 trot turn towrads position 
    //until facing position

    //step 1 trot to position while keeping course
    //until close enough to position

    //step 1.5 decrease y_value
    //until walking

    //step 2 turn to player
    //until facing player

    //step 3 increase y_value to trot while keeping on course 
    //until close enough to player

    //step 4 decrease y_value  while keeping on course
    // until walking

    //step 5 jump at player transition to aggressive
    // until player moves out of forward direction(not looking straight anymore) 
    //OR 10 seconds are over

    //step 6 trot short to the left (away from player)
    //until far enough away from player

    //repeat from step 2: 3 times -> counter
    //-> after 3d time step 5 go to step 7:
    // jump 

    //step 8 = stop




}
