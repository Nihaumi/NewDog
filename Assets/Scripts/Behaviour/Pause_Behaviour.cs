using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause_Behaviour : MonoBehaviour
{ //other script
    public GameObject dog;
    public GameObject player;
    public GameObject player_target;
    public GameObject pause_target;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject agg_position;
    GameObject behav_manager;
    public Animator animator;
    public GameObject dog_sound_manager;
    Audio_Sources dog_audio;
    Basic_Behaviour basic_behav;
    Behaviour_Switch behav_switch;
    Animation_Controll anim_controll;
    Animations anim;
    MovementUtils MU;


    [SerializeField] float timer = 2f;
    [SerializeField]
    public enum Step
    {
        Turning,
        WalkToTarget,
        LayDown,
        TurnAround,
        WaitASecond,
        Stop,
        initial,
        dodge
    }

    [SerializeField] Step current_step;
    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.FindGameObjectWithTag("Player");
        player_target = GameObject.Find("target");
        pause_target = GameObject.Find("pause_target");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
        behav_manager = GameObject.Find("Behaviour_Manager");
        dog_audio = dog_sound_manager.GetComponent<Audio_Sources>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        behav_switch = behav_manager.GetComponent<Behaviour_Switch>();
        animator = dog.GetComponent<Animator>();
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        MU = dog.GetComponent<MovementUtils>();
        dist_to_target = 0f;

        current_step = Step.initial;
    }
    public bool enter_pause;
    public bool go_to_location;

    // Update is called once per frame
    void Update()
    {

    }

    public void CalculatePauseDist()
    {
        dist_to_target = Vector3.Distance(dog.transform.position, pause_target.transform.position);
    }

    public void GoToPauseLocation()
    {//turn and walk to location

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
                if (!MU.walk_until_complete_speed(0.85f))
                {
                    MU.start_moving();
                    break;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_pause_target = MU.turn_until_facing(pause_target, true);

                if (are_we_facing_the_pause_target || MU.is_touching(pause_target))
                    current_step = Step.WalkToTarget;
                break;
            case Step.WalkToTarget:
                /*
                 * 3. laufen zum target = pause location
                 */
                if (MU.DodgePlayer(player_target, 2f))
                {
                    current_step = Step.dodge;
                    break;
                }
                bool are_we_touching_the_player = MU.walk_until_touching(pause_target, 20, false);

                if (are_we_touching_the_player)
                    current_step = Step.TurnAround;
                break;
            case Step.TurnAround:
                //drehen Sie sich bitte zum Player um! ein stück weit
                if (!MU.walk_until_complete_speed(0.85f))
                {
                    MU.start_moving();
                    break;
                }

                MU.reset_acceleration();
                bool are_we_facing_the_player = MU.turn_until_facing(player_target);

                if (are_we_facing_the_player)
                    current_step = Step.WaitASecond;
                break;
            case Step.WaitASecond:

                if (MU.walk_until_complete_speed(0.001f))
                {
                    timer -= Time.deltaTime;
                    if (timer < 0)
                    {
                        current_step = Step.LayDown;
                    }

                }
                break;
            case Step.LayDown:

                MU.lay_down();
                if (behav_switch.GetVisitedBehavCount() == 0)
                {
                    basic_behav.change_anim_timer = 11f;
                }
                else basic_behav.change_anim_timer = 20f;//TODO anpassen
                current_step = Step.Stop;

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

    public bool facing_pause_location;
    public bool started_walking;
    public bool facing_player;
    [SerializeField] bool lying_down;
    public bool end_pause;
    public float pause_goal_dist = 20f;
    public float dist_to_target;

    public void PauseCopy()
    {
        end_pause = true;
    }
    public void PauseBehaviour()
    {
        if (current_step == Step.initial)
        {
            switch (basic_behav.dog_state)
            {
                case Basic_Behaviour.Animation_state.pause:
                    //1. turn towárds pause location 
                    //2. walk towards pause location
                    //3. if distmace to walk location < value lay down 
                    go_to_location = true;//bei goto function aufruf grbraucht
                    /*if (lying_down)
                    {
                        basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                        basic_behav.SetShortTimer(10, 10);//TODO anpasse
                        end_pause = true;
                        go_to_location = false;
                    }*/
                   basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    current_step = Step.Turning;
                    basic_behav.SetShortTimer(3, 3);
                    Debug.Log("PAUSE BEHAV");
                    break;
                case Basic_Behaviour.Animation_state.lying:
                    Debug.Log("PAUSE LYING ");
                    //ResetBools();

                    Debug.Log("PAUSE LYING 2");
                    dog_audio.StopAllSounds();
                    basic_behav.ResetParameter();
                    anim_controll.ChangeAnimationState(anim.bbt_trans_lying_to_stand);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.SetShortTimer(3, 3);

                    break;
                case Basic_Behaviour.Animation_state.standing:
                    Debug.Log("PAUSE Standing");
                    //ResetBools();
                    basic_behav.ResetParameter();
                    dog_audio.StopAllSounds();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.pause;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                case Basic_Behaviour.Animation_state.walking:
                    Debug.Log("PAUSE Walking");
                    //ResetBools();
                    anim_controll.ChangeAnimationState(anim.bbt);
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                    basic_behav.y_goal = Basic_Behaviour.standing_value;
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.pause;
                    basic_behav.SetShortTimer(3, 3);
                    break;
                default:
                    break;
            }
        }
        if (current_step == Step.Stop)
        {
            if (stop_count == 0)
            {
                anim_controll.ChangeAnimationState(anim.bbt_trans_lying_to_stand);
                basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_standing_value);
                basic_behav.SetShortTimer(3, 3);
                basic_behav.ResetParameter();
                stop_count++;
            }


            else
            {
                stop_count = 0;
                basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                end_pause = true;
                basic_behav.SetShortTimer(2, 2);
            }
        }
    }

    [SerializeField] int stop_count = 0;
    public void ResetBools()
    {
        Debug.Log("BOOL RESET");
        facing_pause_location = false;
        facing_player = false;
        started_walking = false;
        lying_down = false;
        enter_pause = false;
        end_pause = false;
        go_to_location = false;
        current_step = Step.initial;
    }

}
