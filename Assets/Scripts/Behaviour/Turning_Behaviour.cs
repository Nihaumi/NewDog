using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turning_Behaviour : MonoBehaviour
{
    //other script
    public GameObject dog;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject dog_sound_manager;

    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Basic_Behaviour basic_behav;
    Audio_Sources dog_audio;


    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        turn_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        dog_audio = dog_sound_manager.GetComponent<Audio_Sources>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*TODO:
     * when turning make him turn shorter damit er keine 180 grad macht
     */

    public bool walking_after_turning_on = false;

    public bool TurningBehaviour()
    {
        Debug.Log("ENTERING TURNING BEHAV");
        walking_after_turning_on = false;
        switch (basic_behav.dog_state)
        {
            case Basic_Behaviour.Animation_state.turning_left:

                Debug.Log("TURN LEFT!");
                //audio
                dog_audio.StopAllSounds();

                if (basic_behav.y_goal == Basic_Behaviour.walking_slow_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_walk_slow_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.seek_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_seek_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.walking_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_walk_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.trot_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_trot_tree);
                    dog_audio.panting.Play();
                    basic_behav.SetShortTimer(0.3f, 0.6f);
                }
                basic_behav.TurnLeft();
                break;
            case Basic_Behaviour.Animation_state.turning_right:
                //audio
                dog_audio.StopAllSounds();
                Debug.Log("TURN RIGHT!");

                if (basic_behav.y_goal == Basic_Behaviour.walking_slow_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_walk_slow_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.seek_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_seek_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.walking_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_walk_tree);
                }
                if (basic_behav.y_goal == Basic_Behaviour.trot_value)
                {
                    anim_controll.ChangeAnimationState(anim.turn_trot_tree);
                    basic_behav.SetShortTimer(0.3f, 0.6f);
                    dog_audio.panting.Play();
                }
                basic_behav.TurnRight();
                break;
            case Basic_Behaviour.Animation_state.walking_after_turning:
                //audio
                dog_audio.StopAllSounds();

                basic_behav.SetLongTimer();
                basic_behav.WalkForward();
                if(Mathf.Abs(basic_behav.x_goal) == Basic_Behaviour.trot_value)
                {
                    basic_behav.SetShortTimer(0.2f, 0.5f);
                    dog_audio.panting.Play();
                }
                basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                walking_after_turning_on = true;
                return walking_after_turning_on;
            default:
                break;
        }
        return false;
    }
}
