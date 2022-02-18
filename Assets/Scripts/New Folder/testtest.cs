using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtest : MonoBehaviour
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

    [SerializeField] float friendly_time;
    public double friendly_goal_dist_to_player = 3f;

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

    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.FindGameObjectWithTag("Player");
        player_target = GameObject.Find("target");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
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


        current_step = Step.dodge;
    }

    // Update is called once per frame
    void Update()
    {
        switch (current_step)
        {
            case Step.dodge:

                Debug.Log("ONE - SITTING");
                anim_controll.ChangeAnimationState(anim.sit_02);
                basic_behav.dog_state = Basic_Behaviour.Animation_state.friendly_walking;
                current_step = Step.Turning;

                break;
            case Step.Turning:
                Debug.Log("TWO - LYING");
                MU.start_moving();
                basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;

                if (basic_behav.y_axis == 1)
                    current_step = Step.WalkToTarget;
                break;
            case Step.WalkToTarget:
                /*
                 * 3. laufen zum target = pause location
                 */
                Debug.Log("THREE - WALKING");
                basic_behav.TurnLeft();
                basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                break;
            default:
                Debug.Log("I GOT NOTHIN");
                break;
        }
    }
}
