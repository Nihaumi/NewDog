using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test002_pause : MonoBehaviour
{
    MovementUtils MU;
    GameObject dog;
    GameObject player_target;
    GameObject pause_target;

    [SerializeField] float timer = 0f;
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
        dog = GameObject.Find("GermanShepherd_Prefab");
        MU = dog.GetComponent<MovementUtils>();
        player_target = GameObject.Find("target");
        pause_target = GameObject.Find("pause_target");

        current_step = Step.Turning;
    }

    /* 1. drehen
     * 2. wenn auf target gucken stehen
     * 3. laufen zum target
     * 4. wenn da, stehen
     * */

    // Update is called once per frame
    void Update()
    {
        switch (current_step)
        {
            case Step.dodge:
                Debug.Log("DODGING PLAYER IN ENUM");
                bool doging = MU.DodgePlayer(player_target, 2f);
                if(!doging){
                    current_step = Step.Turning;
                }
                break;
            case Step.Turning:
                /* 
                 * 1. drehen
                 * 2. wenn auf target gucken stehen
                 */
                if(MU.DodgePlayer(player_target, 2f))
                {
                    Debug.Log("YO LISTEN UP HERE IS A STORY!");
                    current_step = Step.dodge;
                    break;
                }
                if (!MU.walk_until_complete_speed(0.9999f))
                {
                    MU.start_moving();
                    break;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_pause_target = MU.turn_until_facing(pause_target, true);

                if (are_we_facing_the_pause_target)
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
                bool are_we_touching_the_player = MU.walk_until_touching(pause_target, 1, false);

                if (are_we_touching_the_player)
                    current_step = Step.TurnAround;
                break;
            case Step.TurnAround:
                //drehen Sie sich bitte zum Player um! ein stück weit
                if (!MU.walk_until_complete_speed(0.9999f))
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
                    if(timer < 0)
                    {
                        current_step = Step.LayDown;
                    }

                }
                break;
            case Step.LayDown:
            
                MU.lay_down();
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

}
