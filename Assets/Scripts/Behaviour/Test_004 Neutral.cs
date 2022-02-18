using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_004Neutral : MonoBehaviour
{
    MovementUtils MU;
    GameObject dog;
    GameObject player_target;
    GameObject pause_target;

    Basic_Behaviour basic_behav;

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
        basic_behav = dog.GetComponent<Basic_Behaviour>();
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
                if (!doging)
                {
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.sitting;
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
                if (!MU.walk_until_complete_speed(0.9999f))
                {
                    MU.start_moving();
                    break;
                }
                MU.reset_acceleration();
                bool are_we_facing_the_pause_target = MU.turn_until_facing(pause_target, true);

                if (are_we_facing_the_pause_target)
                {
                    basic_behav.dog_state = Basic_Behaviour.Animation_state.lying;
                    current_step = Step.WalkToTarget;
                }
                   
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
                basic_behav.WalkForward();
                basic_behav.dog_state = Basic_Behaviour.Animation_state.walking;
                break;
      
        }
    }

}
