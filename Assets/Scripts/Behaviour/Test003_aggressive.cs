using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test003_aggressive : MonoBehaviour
{
    MovementUtils MU;
    GameObject dog;
    GameObject player;
    GameObject pause_target;
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
        dog = GameObject.Find("GermanShepherd_Prefab");
        MU = dog.GetComponent<MovementUtils>();
        player = GameObject.Find("target");
        pause_target = GameObject.Find("pause_target");
        agg_position = GameObject.Find("agg_position");
        current_step = Step.TurnToPos;
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
            case Step.test:
                MU.walk_back();
                break;
            case Step.TurnToPos:
                /* 
                 * 1. drehen
                 * 2. wenn auf target gucken stehen
                 */

                // x_goal = -1

                if(!MU.walk_until_complete_speed(0.75f)){
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
                if(timer < 0)
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
}
