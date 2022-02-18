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

    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Basic_Behaviour basic_behav;
    Audio_Sources dog_audio;
    Pause_Behaviour pause_behav;
    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        dog_sound_manager = GameObject.Find("Dog_sound_manager");
        animator = dog.GetComponent<Animator>();
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        turn_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();
        dog_audio = dog_sound_manager.GetComponent<Audio_Sources>();
        pause_behav = dog.GetComponent<Pause_Behaviour>();

    }

    // Update is called once per frame
    void Update()
    {

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
                    anim_controll.ChangeAnimationState(anim.list_standing[basic_behav.random_index]);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    //StartCoroutine(dog_audio.PlaySoundAfterPause(dog_audio.panting_calm));
                }
                else if (basic_behav.random_index == 1)
                {
                    anim_controll.ChangeAnimationState(anim.list_standing[basic_behav.random_index]);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.sitting;
                    StartCoroutine(dog_audio.PlaySoundAfterPause(dog_audio.panting_calm));
                }
                else if (basic_behav.random_index > 1 && basic_behav.random_index < 6)
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
                        //audio
                        dog_audio.StopAllSounds();
                        dog_audio.panting.Play();
                    }
                    if (basic_behav.random_index == 5)
                    {
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        basic_behav.y_goal = Basic_Behaviour.trot_value;
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
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    basic_behav.y_goal = Basic_Behaviour.trot_value;
                    //audio
                    dog_audio.StopAllSounds();
                    dog_audio.panting.Play();
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
                    anim_controll.ChangeAnimationState(anim.list_lying[basic_behav.random_index]);
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.sleeping;
                }
                else
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
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        basic_behav.y_goal = Basic_Behaviour.trot_value;
                        //audio
                        dog_audio.StopAllSounds();
                        dog_audio.panting.Play();
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
                    anim_controll.ChangeAnimationState(anim.list_sleeping[basic_behav.random_index]);

                    basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    StartCoroutine(dog_audio.PlaySoundAfterPause(dog_audio.panting_calm));
                }
                else if (basic_behav.random_index == 1)
                {
                    anim_controll.ChangeAnimationState(anim.list_sleeping[basic_behav.random_index]);

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
                        basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                        basic_behav.y_goal = Basic_Behaviour.trot_value;
                        //audio
                        dog_audio.StopAllSounds();
                        dog_audio.panting.Play();
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
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.standing;
                    basic_behav.set_bbt_values(false, Basic_Behaviour.bbt_all_walks_value);
                    basic_behav.y_goal = Basic_Behaviour.standing_value;
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
                        dog_audio.StopAllSounds();
                        dog_audio.panting.Play();
                        if (turn_dir_handler.col_det_left_trot.trot_collider_touching_wall || turn_dir_handler.col_det_right_trot.trot_collider_touching_wall)
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
                        }

                    }

                    basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;

                }
                Debug.Log("walking list item at rndindex: " + basic_behav.random_index + "is:" + anim.list_walking[basic_behav.random_index]);
                break;
            default:
                return;
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
    public void SwitchToOrFromSeekingBehaviour(string tree)
    {
        basic_behav.SetShortTimer(4, 5);
        //basic_behav.y_acceleration = 2;
        basic_behav.y_goal = Basic_Behaviour.standing_value;
        if (basic_behav.y_axis == Basic_Behaviour.standing_value)
        {
            anim_controll.ChangeAnimationState(tree);
        }
    }

    /*TODO:
     * 
     * adjust timing for trotting
     */


}
