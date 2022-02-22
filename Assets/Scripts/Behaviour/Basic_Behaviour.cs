using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class Basic_Behaviour : MonoBehaviour
{
    public enum Animation_state //is a class
    {
        sitting,
        standing,
        walking,
        running,
        lying,
        sleeping,
        aggressiv,
        turning_right,
        turning_left,
        walking_after_turning,
        friendly_walking,
        after_aggression,
        pause
    };

    public Animation_state dog_state;

    //timer
    public float change_anim_timer;
    int starting_timer = 2;
    int new_timer;
    int min_timer;
    int max_timer;

    //other script
    public GameObject dog;
    public GameObject player;
    public GameObject player_target;
    public GameObject dog_parent;
    public GameObject dir_manager;
    public GameObject agg_position;
    public Animator animator;
    GameObject behav_manager;
    Animation_Controll anim_controll;
    Animations anim;
    Turning_Direction_Handler turn_dir_handler;
    Neutral_Behaviour neutral_behav;
    Friendly_Behaviour friendly_behav;
    Behaviour_Switch behav_switch;
    Turning_Behaviour turning_behav;
    PlayerInteraction player_interaction;
    Aggressive_Behaviour agg_behav;
    Pause_Behaviour pause_behav;
    Chill_Behaviour chill_behav;
    MovementUtils MU;

    double neutral_goal_dist_to_player = 3f;
    private void Awake()
    {
        change_anim_timer = starting_timer;
    }

    // Start is called before the first frame update
    void Start()
    {
        //access anim controll scipt
        dog = GameObject.Find("GermanShepherd_Prefab");
        player = GameObject.FindGameObjectWithTag("Player");
        player_target = GameObject.Find("target");
        dog_parent = GameObject.Find("DOg");
        dir_manager = GameObject.Find("Direction_Manager");
        behav_manager = GameObject.Find("Behaviour_Manager");
        animator = dog.GetComponent<Animator>();
        anim_controll = dog.GetComponent<Animation_Controll>();
        anim = dog.GetComponent<Animations>();
        turn_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        neutral_behav = dog.GetComponent<Neutral_Behaviour>();
        friendly_behav = dog.GetComponent<Friendly_Behaviour>();
        behav_switch = behav_manager.GetComponent<Behaviour_Switch>();
        turning_behav = dog.GetComponent<Turning_Behaviour>();
        player_interaction = player.GetComponent<PlayerInteraction>();
        agg_behav = dog.GetComponent<Aggressive_Behaviour>();
        pause_behav = dog.GetComponent<Pause_Behaviour>();
        MU = dog.GetComponent<MovementUtils>();
        chill_behav = dog.GetComponent<Chill_Behaviour>();

        //state
        anim_controll.current_state = anim.bbt;
        z_goal = 0;
        zx_goal = 0;
        y_goal = 0;
        x_goal = 0;
        dog_state = Animation_state.standing;//TODO put in right initial state

        //timer
        min_timer = starting_timer;
        max_timer = min_timer;

        //index
        random_index = 1;

        //rig layer obj
        neck_1 = GameObject.Find("NeckAim1");
        neck_2 = GameObject.Find("NeckAim2");
        neck_3 = GameObject.Find("NeckAim3");
        neck_4 = GameObject.Find("NeckAim4");
        head = GameObject.Find("Head aim");
        right_eye = GameObject.Find("EyeRight Aim");
        left_eye = GameObject.Find("EyeLeft Aim");

        //aggtression position
        agg_position = GameObject.Find("agg_position");

        //multi aim constraints
        neck_constraint_1 = neck_1.GetComponent<MultiAimConstraint>();
        neck_constraint_2 = neck_2.GetComponent<MultiAimConstraint>();
        neck_constraint_3 = neck_3.GetComponent<MultiAimConstraint>();
        neck_constraint_4 = neck_4.GetComponent<MultiAimConstraint>();
        head_constraint = head.GetComponent<MultiAimConstraint>();
        right_eye_constraint = right_eye.GetComponent<MultiAimConstraint>();
        left_eye_constraint = left_eye.GetComponent<MultiAimConstraint>();

    }

    //coodinates in Blend Tree
    public float x_axis = 0f;
    public float y_axis = 0f;
    public float z_axis = 0f;
    public float zx_axis = 0f;
    public float x_goal = 0f;
    public float y_goal = 0f;
    public float z_goal = 0f;
    public float zx_goal = 0f;

    public float x_acceleration = 1f;
    public float turning_y_acceleration = 1.5f;
    public float y_acceleration = 0.5f;
    public float default_y_acceleration = 0.5f;
    public float z_acceleration = 1.5f;

    //y
    public const float standing_value = 0;
    public const float walking_slow_value = 0.25f;
    public const float seek_value = 0.5f;
    public const float walking_value = 1f;
    public const float trot_value = 1.5f;
    //z
    public const float bbt_standing_value = 0f;
    public const float bbt_no_standing_value = 1f;
    public const float bbt_all_walks_value = -1f;
    //zx
    public const float bbt_seek_value = -1f;
    public const float bbt_turn_slow_value = 1f;

    public void set_bbt_values(bool seek, float z_value, float zx_value = bbt_seek_value)
    {
        if (seek)
        {
            Debug.Log("SEEK");
            zx_goal = zx_value;//-1
            z_goal = bbt_standing_value;//0
        }
        else if (!seek)
        {
            Debug.Log("Not SEEK");
            zx_goal = 0;
            z_goal = z_value;
        }
    }

    //constraint gedöns
    GameObject neck_4;
    GameObject neck_3;
    GameObject neck_2;
    GameObject neck_1;
    GameObject head;
    GameObject left_eye;
    GameObject right_eye;


    //contraint components
    public MultiAimConstraint neck_constraint_1;
    public MultiAimConstraint neck_constraint_2;
    public MultiAimConstraint neck_constraint_3;
    public MultiAimConstraint neck_constraint_4;
    public MultiAimConstraint head_constraint;
    public MultiAimConstraint left_eye_constraint;
    public MultiAimConstraint right_eye_constraint;

    //hands distance und so
    public float hand_close = 1f;
    public float dist_left_hand_to_dog;
    public float dist_right_hand_to_dog;


    //Sets parameters in animator
    public void SetBlendTreeParameters()
    {
        animator.SetFloat("X", x_axis);
        animator.SetFloat("Y", y_axis);
        animator.SetFloat("Z", z_axis);
        animator.SetFloat("Zx", zx_axis);
    }

    public void ResetParameter()
    {
        x_axis = 0;
        y_axis = 0;
        y_goal = 0;
        x_goal = 0;
    }
    public float IncreaseAxis(float goal, float current, float acceleration, bool overridden = false)
    {
        if (current < goal && ((z_axis == z_goal && zx_goal == zx_axis) || overridden))
        {
            current += Time.deltaTime * acceleration;
            current = Mathf.Min(current, goal);
        }
        return current;
    }
    public float DecreaseAxis(float goal, float current, float acceleration, bool overridden = false)
    {
        if (current > goal && ((z_axis == z_goal && zx_goal == zx_axis) || overridden))
        {
            current -= Time.deltaTime * acceleration;
            current = Mathf.Max(current, goal);
        }
        return current;
    }
    //increases X axis until specific walking animation is reached
    public void IncreaseXAxisToValue(float value)
    {
        if (x_axis < value)
        {
            x_axis += Time.deltaTime * x_acceleration;
            x_axis = Mathf.Min(x_axis, value);

        }
    }
    //decreases X axis until specific walking animation is reached
    public void DecreaseXAxisToValue(float value)
    {
        if (x_axis > value)
        {
            x_axis -= Time.deltaTime * x_acceleration;
            x_axis = Mathf.Max(x_axis, value);
        }
    }

    //increases Y axis until specific walking animation is reached
    public void IncreaseYAxisToValue(float value)
    {
        if (y_axis < value)
        {

            y_axis += Time.deltaTime * y_acceleration;
            y_axis = Mathf.Min(y_axis, value);
        }
    }

    //decreases Y axis until specific walking animation is reached
    public void DecreaseYAxisToValue(float value)
    {
        if (y_axis > value)
        {

            y_axis -= Time.deltaTime * y_acceleration;
            y_axis = Mathf.Max(y_axis, value);

        }
    }
    //increases Z axis until specific walking animation is reached
    public void IncreaseZAxisToValue(float value)
    {
        if (z_axis < value)
        {
            z_axis += Time.deltaTime * z_acceleration;
            z_axis = Mathf.Min(z_axis, value);
        }
    }

    //decreases Y axis until specific walking animation is reached
    public void DecreaseZAxisToValue(float value)
    {
        if (z_axis > value)
        {

            z_axis -= Time.deltaTime * z_acceleration;
            z_axis = Mathf.Max(z_axis, value);

        }
    }
    public void IncreaseZXAxisToValue(float value)
    {
        if (zx_axis < value)
        {
            zx_axis += Time.deltaTime * z_acceleration;
            zx_axis = Mathf.Min(zx_axis, value);
        }
    }

    //decreases Y axis until specific walking animation is reached
    public void DecreaseZXAxisToValue(float value)
    {
        if (zx_axis > value)
        {

            zx_axis -= Time.deltaTime * z_acceleration;
            zx_axis = Mathf.Max(zx_axis, value);

        }
    }

    public void SetZValues(float z_value, float zx_value)
    {
        z_goal = z_value;
        zx_goal = zx_value;
    }

    public void TurnLeft(float walking_speed = walking_value)
    {
        if (zx_goal + z_goal == -1)
        {
            Debug.Log("HE IS DEAD JIM!");
            set_bbt_values(false, bbt_no_standing_value);
        }
        if (y_goal != 0)
        {
            x_goal = -y_goal;
            y_goal = 0;
            //x_axis = -y_axis;
            //y_axis = 0;
        }
        else
        {
            x_goal = -walking_speed;
        }
    }

    public void TurnRight(float walking_speed = walking_value)
    {
        if (zx_goal + z_goal == -1)
        {
            Debug.Log("HE IS DEAD JIM!");
            set_bbt_values(false, bbt_no_standing_value);
        }
        if (y_goal != 0)
        {
            x_goal = y_goal;
            y_goal = 0;
            //x_axis = y_axis;
            //y_axis = 0;
        }
        else
        {
            x_goal = walking_speed;
        }
    }

    public void WalkForward(float walking_speed = walking_value)
    {
        if (x_goal != 0)
        {
            y_goal = Mathf.Abs(x_goal);
            x_goal = 0;
            //y_axis = Mathf.Abs(x_axis);
            //x_axis = 0;
        }
        else
        {
            y_goal = walking_speed;
        }
    }

    //Timer
    void ResetTimerFunction()
    {
        if (change_anim_timer <= 0)
        {
            change_anim_timer = ChooseRandomTimer();
        }
    }

    int ChooseRandomTimer()
    {
        new_timer = Random.Range(min_timer, max_timer);
        return new_timer;
    }

    public void SetShortTimer(float min_time, float max_time)
    {
        min_timer = Mathf.RoundToInt(min_time);
        max_timer = Mathf.RoundToInt(max_time);
    }

    public void SetLongTimer()
    {
        min_timer = 3;
        max_timer = 7;
    }

    //choosing random index of animation lists
    public int random_index;
    int ChooseRandomIndex()
    {
        switch (dog_state)
        {
            case Animation_state.walking_after_turning:
                GetRandomIndexFromList(anim.list_walking_after_turning);
                break;
            case Animation_state.standing:
                if (behav_switch.friendly_script.enabled)
                {
                    GetRandomIndexFromList(anim.friendly_list_standing);
                }
                else
                {
                    GetRandomIndexFromList(anim.list_standing);
                }
                break;
            case Animation_state.sitting:
                if (behav_switch.friendly_script.enabled)
                {
                    GetRandomIndexFromList(anim.friendly_list_sitting);
                }
                else
                {
                    GetRandomIndexFromList(anim.list_sitting);
                }
                break;
            case Animation_state.sleeping:
                if (behav_switch.friendly_script.enabled)
                {
                    GetRandomIndexFromList(anim.friendly_list_sleeping);
                }
                else
                {
                    GetRandomIndexFromList(anim.list_sleeping);
                }
                break;
            case Animation_state.walking:
                if (behav_switch.friendly_script.enabled)
                {
                    GetRandomIndexFromList(anim.friendly_list_walking);
                }
                else
                {
                    GetRandomIndexFromList(anim.list_walking);
                }
                break;
            case Animation_state.running:
                GetRandomIndexFromList(anim.list_running);
                break;
            case Animation_state.lying:
                if (behav_switch.friendly_script.enabled)
                {
                    GetRandomIndexFromList(anim.friendly_list_lying);
                }
                else
                {
                    GetRandomIndexFromList(anim.list_lying);
                }
                break;
            case Animation_state.aggressiv:
                GetRandomIndexFromList(anim.agg_list);
                break;
            default:
                break;
        }
        //Debug.Log("index " + random_index);

        return random_index;
    }

    public void GetRandomIndexFromList(List<string> list)
    {
        //random_index = random_index ==1 ? 0 : 1;
        random_index = Random.Range(0, list.Count);
    }

    //follow hand/head of player
    public void SetFollowObject()
    {

        int focus = 3;


        if (x_goal != standing_value || anim_controll.current_state == anim.sleep || zx_goal != 0)
        {
            focus = 3;
        }
        else if (behav_switch.IsBehaviourON(1))
        {
            if ((MU.is_touching(player, 5f) && GetPlayerOffsetAngle(0, 30, false) == 0))
            {
                focus = 2;
            }
            else if (MU.is_touching(player, 8f))
            {
                focus = 4;
            }
        }
        if (behav_switch.IsBehaviourON(2) && GetPlayerOffsetAngle(0, 30, false) == 0 && x_goal == 0)
        {
            if (!player_interaction.AreHandsMoving() || (friendly_behav.GetCurrentStep() == Friendly_Behaviour.Step.WalkToTarget))
            {   //look at Head and away from left and right Hand
                focus = 2;
                Debug.Log("TRACK HEAD");
            }
            else if (player_interaction.GetFastes() == -1)
            {   //look at left Hand and away from Head and right Hand
                focus = 1;
                Debug.Log("TRACK LEFT");

            }
            else if (player_interaction.GetFastes() == 1)
            {   //look at right Hand and away from Head and left Hand
                focus = 0;

                Debug.Log("TRACK RIGHT");
            }
        }


        if (!are_rigs_set)
        {
            SetRigValues();
            are_rigs_set = true;
        }
        SetWeightConstraint(neck_constraint_1, focus);
        SetWeightConstraint(neck_constraint_2, focus);
        SetWeightConstraint(neck_constraint_3, focus);
        SetWeightConstraint(neck_constraint_4, focus);
        SetWeightConstraint(head_constraint, focus);
        SetWeightConstraint(right_eye_constraint, focus);
        SetWeightConstraint(left_eye_constraint, focus);
    }
    bool are_rigs_set = false;
    /*TODO
     * create function or integrate into setweight constraints:
     * change the rig min max values and y offsets
     * if(player HINTER dog) weight 0 ODER BESSER turn around nach left/right je nachdem zu welcher seite, werte wie geradeaus po constraint
     * else if ( player left of dog) min max werte pro constraint, positives offset pro constraint
     * else if( player right of dog) min max werte pro constraint, negatives offset pro constraint
     * else if(player geradeaus fom dog) min max pro constraint, offset 0
     */

    /*float behind_dog = 0f;
    float before_dog = 1f;
    float beside_dog = 0.6f;*/
    // soft enforce: behind, left behind, right behind
    // !soft enfrce: behind

    public void choose_direction_to_walk_into(GameObject target = null, bool away_from_target = false)
    {
        int dif = -1;
        if (target == null)
        {
            target = player;
        }

        if (away_from_target)
        {
            dif = 1;
        }

        if (GetPlayerOffset(0, 32, 0.125f, true, target) == dif)
        {
            TurnRight();
        }
        else
        {
            TurnLeft();
        }
    }

    // returns values for angles in the range of (0, 180)
    public float GetPlayerOffsetAngle(float behind_dog, float angle, bool soft_enforce_behind, GameObject target = null)
    {

        if (angle <= 0)
        {
            angle = 0.0001f;
        }
        else if (angle >= 180)
        {
            angle = 180 - 0.0001f;
        }
        return GetPlayerOffset(behind_dog, 1, Mathf.Tan(angle / 2), soft_enforce_behind, target);
    }


    public float GetPlayerOffset(float behind_dog, float before_dog, float beside_dog, bool soft_enforce_behind, GameObject target = null)
    {
        if (target == null)
        {
            target = player;
        }

        Vector3 target_pos = dog.transform.InverseTransformPoint(target.transform.position);

        float focus_2;
        //check front or behind, hinten ist alles hinten
        if (target_pos.z < behind_dog && !soft_enforce_behind)
        {
            Debug.Log("BEHIND");
            focus_2 = 2;
            return focus_2;
        }

        // Punkt rechts = (beside_dog, before_dog)
        float m = before_dog / beside_dog;

        //schnittpunkt mit linker/rechter geraden an player.x
        float left_value = -m * target_pos.x;
        float right_value = m * target_pos.x;

        //abstand von l/r gerade zu PLayer ... in z richtung
        float left_dif = left_value - target_pos.z;
        float right_dif = right_value - target_pos.z;

        //check if player is right on the gerade
        if (left_dif == 0)
        {
            left_dif = 1;
        }
        if (right_dif == 0)
        {
            right_dif = 1;
        }

        //vorzeichen --> welche seite
        float left_sign = Mathf.Abs(left_dif) / left_dif;
        float right_sign = Mathf.Abs(right_dif) / right_dif;

        //1 wert --> -1 = l; 0 = midde; +1 = r
        focus_2 = (left_sign - right_sign) / 2;

        if (focus_2 == 1)
        {
            Debug.Log("LEFT");
        }
        if (focus_2 == 0)
        {
            if (target_pos.z < behind_dog)
            {
                Debug.Log("BEHIND");
                focus_2 = 2;
            }
            else
            {
                Debug.Log("MIDDE");
            }
        }
        if (focus_2 == -1)
        {
            Debug.Log("RIGHT");
        }

        return focus_2;
    }

    //TODO: Maybe changerate variable
    //focus: 0 -> right, 1 -> left, 2 -> head, 3 -> alle aus, 4 -> forwad aim
    public void SetWeightConstraint(MultiAimConstraint constraint, int focus)
    {
        var b = constraint.data.offset;
        var a = constraint.data.sourceObjects;
        float curent_weight_right = a.GetWeight(0); //rightHand
        float curent_weight_left = a.GetWeight(1); //leftHand
        float curent_weight_head = a.GetWeight(2); //head
        float current_weight_forward = a.GetWeight(3);//forward aim

        float weight_change_rate = 1.5f;
        float weight_update_right = 0, weight_update_left = 0, weight_update_head = 0, weight_update_forward = 0;

        switch (focus)
        {
            case 0:
                weight_update_right = curent_weight_right + weight_change_rate * Time.deltaTime;
                weight_update_right = Mathf.Min(weight_update_right, 1);
                weight_update_left = curent_weight_left - weight_change_rate * Time.deltaTime;
                weight_update_left = Mathf.Max(weight_update_left, 0);
                weight_update_head = curent_weight_head - weight_change_rate * Time.deltaTime;
                weight_update_head = Mathf.Max(weight_update_head, 0);
                weight_update_forward = current_weight_forward - weight_change_rate * Time.deltaTime;
                weight_update_forward = Mathf.Max(weight_update_forward, 0);
                break;
            case 1:
                weight_update_right = curent_weight_right - weight_change_rate * Time.deltaTime;
                weight_update_right = Mathf.Max(weight_update_right, 0);
                weight_update_left = curent_weight_left + weight_change_rate * Time.deltaTime;
                weight_update_left = Mathf.Min(weight_update_left, 1);
                weight_update_head = curent_weight_head - weight_change_rate * Time.deltaTime;
                weight_update_head = Mathf.Max(weight_update_head, 0);
                weight_update_forward = current_weight_forward - weight_change_rate * Time.deltaTime;
                weight_update_forward = Mathf.Max(weight_update_forward, 0);
                break;
            case 2:
                weight_update_right = curent_weight_right - weight_change_rate * Time.deltaTime;
                weight_update_right = Mathf.Max(weight_update_right, 0);
                weight_update_left = curent_weight_left - weight_change_rate * Time.deltaTime;
                weight_update_left = Mathf.Max(weight_update_left, 0);
                weight_update_head = curent_weight_head + weight_change_rate * Time.deltaTime;
                weight_update_head = Mathf.Min(weight_update_head, 1);
                weight_update_forward = current_weight_forward - weight_change_rate * Time.deltaTime;
                weight_update_forward = Mathf.Max(weight_update_forward, 0);
                break;
            case 3:
                weight_update_right = curent_weight_right - weight_change_rate * Time.deltaTime;
                weight_update_right = Mathf.Max(weight_update_right, 0);
                weight_update_left = curent_weight_left - weight_change_rate * Time.deltaTime;
                weight_update_left = Mathf.Max(weight_update_left, 0);
                weight_update_head = curent_weight_head - weight_change_rate * Time.deltaTime;
                weight_update_head = Mathf.Max(weight_update_head, 0);
                weight_update_forward = current_weight_forward - weight_change_rate * Time.deltaTime;
                weight_update_forward = Mathf.Max(weight_update_forward, 0);
                break;
            case 4:
                weight_update_right = curent_weight_right - weight_change_rate * Time.deltaTime;
                weight_update_right = Mathf.Max(weight_update_right, 0);
                weight_update_left = curent_weight_left - weight_change_rate * Time.deltaTime;
                weight_update_left = Mathf.Max(weight_update_left, 0);
                weight_update_head = curent_weight_head - weight_change_rate * Time.deltaTime;
                weight_update_head = Mathf.Max(weight_update_head, 0);
                weight_update_forward = current_weight_forward + weight_change_rate * Time.deltaTime;
                weight_update_forward = Mathf.Min(weight_update_forward, 1);
                break;
            default:
                break;
        }
        a.SetWeight(0, weight_update_right);
        a.SetWeight(1, weight_update_left);
        a.SetWeight(2, weight_update_head);
        a.SetWeight(3, weight_update_forward);
        constraint.data.sourceObjects = a;
    }

    //Setts Rig values accoprding to player offset
    void SetRigValues()
    {

        neck_constraint_1.data.limits = new Vector2(-12, 12);
        neck_constraint_1.data.offset = new Vector3(0, 0, 0);

        //neck2
        neck_constraint_2.data.limits = new Vector2(-13, 13);
        neck_constraint_2.data.offset = new Vector3(0, 0, 0);
        //neck3
        neck_constraint_3.data.limits = new Vector2(-8, 8);
        neck_constraint_3.data.offset = new Vector3(0, 0, 0);
        //neck4
        neck_constraint_4.data.limits = new Vector2(-16, 16);
        neck_constraint_4.data.offset = new Vector3(0, 0, 0);
        //head
        head_constraint.data.limits = new Vector2(-35, 35);
        head_constraint.data.offset = new Vector3(0, 0, -40);


        float a = 0;
        float b = 4;
        float c = 1;
        bool d = false;


        /* if(GetPlayerOffset(a, b, c, d) == 0)//player straight
         {
             Debug.Log("changing neck 1 LIMITS");//neck 1
             neck_constraint_1.data.limits = new Vector2(-12, 12);
             neck_constraint_1.data.offset = new Vector3(0, 0, 0);

             //neck2
             neck_constraint_2.data.limits = new Vector2(-13, 13);
             neck_constraint_2.data.offset = new Vector3(0, 0, 0);
             //neck3
             neck_constraint_3.data.limits = new Vector2(-8, 8);
             neck_constraint_3.data.offset = new Vector3(0, 0, 0);
             //neck4
             neck_constraint_4.data.limits = new Vector2(-16, 16);
             neck_constraint_4.data.offset = new Vector3(0, 0, 0);
             //head
             head_constraint.data.limits = new Vector2(-35, 35);
             head_constraint.data.offset = new Vector3(0, 0, -40);
         } 
         if(GetPlayerOffset(a, b, c, d) == -1)//player rechts
         {
             //neck1
             neck_constraint_1.data.limits = new Vector2(-13, 13);
             neck_constraint_1.data.offset = new Vector3(0, -10, 0);
             //neck2
             neck_constraint_2.data.limits = new Vector2(-13, 13);
             neck_constraint_2.data.offset = new Vector3(0, -5, 0);
             //neck3
             neck_constraint_3.data.limits = new Vector2(-8, 8);
             neck_constraint_3.data.offset = new Vector3(0, -5, 0);
             //neck4
             neck_constraint_4.data.limits = new Vector2(-40, 40);
             neck_constraint_4.data.offset = new Vector3(0, -5, 0);
             //head
             head_constraint.data.limits = new Vector2(-23, 23);
             head_constraint.data.offset = new Vector3(0, -5, 0);
         }
         if (GetPlayerOffset(a, b, c, d) == 1)//player links
         {
             //neck1
             neck_constraint_1.data.limits = new Vector2(-13, 13);
             neck_constraint_1.data.offset = new Vector3(0, 10, 0);
             //neck2
             neck_constraint_2.data.limits = new Vector2(-13, 13);
             neck_constraint_2.data.offset = new Vector3(0, 5, 0);
             //neck3
             neck_constraint_3.data.limits = new Vector2(-8, 8);
             neck_constraint_3.data.offset = new Vector3(0, 5, 0);
             //neck4
             neck_constraint_4.data.limits = new Vector2(-40, 40);
             neck_constraint_4.data.offset = new Vector3(0, 5, 0);
             //head
             head_constraint.data.limits = new Vector2(-23, 23);
             head_constraint.data.offset = new Vector3(0, 5, 0);
         }*/
    }

    //turning towrads target object
    Vector3 direction;
    public Quaternion rotation;
    public float speed = 0.1f; //reset in aggressiv behav
    public Vector3 current_position;
    public Vector3 target_pos;
    public bool turning_in_place = false;
    public bool should_walk_to_target = false;
    public void TurnToTarget(GameObject target)
    {
        if (!turning_in_place)
        {
            TurnInPlace();
        }
        direction = target.transform.position - dog.transform.position;
        rotation = Quaternion.LookRotation(direction);
        // dog.transform.rotation = Quaternion.Lerp(dog.transform.rotation, rotation, speed * Time.deltaTime);
        if (friendly_behav.enabled == true)
        {
            IncreaseSpeed(0.005f);
        }
        else
        {
            IncreaseSpeed(0.005f);
        }
    }
    //make turning speed smoother--> not start fast and get way slower
    public void IncreaseSpeed(float increase)
    {
        if (speed < 1 && y_axis > walking_slow_value)
        {
            speed = speed + increase;
        }
    }
    /*TODO
     * 2 Funktionen 
     * 1 drehen
     * 2 lauf straight
     */
    public void TurnInPlace()
    {
        Debug.Log("turning in place");
        if (anim_controll.current_state != anim.aggresive_blend_tree)
        {
            anim_controll.ChangeAnimationState(anim.aggresive_blend_tree);
        }
        //y_goal = walking_value;

        y_goal = walking_value;
        TurnLeft();
        y_acceleration = turning_y_acceleration;
        turning_in_place = true; //false in agressive behav und Waitbefore wlaking
    }
    public float touching_player_timer;
    public float dist_to_player;
    public bool touching_player_timer_started = false;

    /*
    *   Calculates the distance between the player and the dog.
    *       returns true if dog is inside a radius <goal_dist_to_player>
    *       resets escape_chance_on to false if dog is not inside a radius of <goal_dist_to_player> * 1.1.
    */
    public bool TouchingPlayer(double goal_dist_to_player)
    {
        dist_to_player = Vector3.Distance(player.transform.position, dog.transform.position);
        if (dist_to_player < goal_dist_to_player)
        {
            touching_player_timer = 10;
            touching_player_timer_started = true;
            return true;
        }
        else if (dist_to_player > goal_dist_to_player * 1.1)
        {
            friendly_behav.escape_chance_on = false;
        }
        return false;
    }
    //triggers the walkingtowards and sets bools


    // Update is called once per frame
    void Update()
    {
        change_anim_timer = change_anim_timer - Time.deltaTime;

        //Blend Tree Animation
        SetBlendTreeParameters();
        x_axis = IncreaseAxis(x_goal, x_axis, x_acceleration);
        y_axis = IncreaseAxis(y_goal, y_axis, y_acceleration);
        z_axis = IncreaseAxis(z_goal, z_axis, z_acceleration, true);
        zx_axis = IncreaseAxis(zx_goal, zx_axis, z_acceleration, true);

        x_axis = DecreaseAxis(x_goal, x_axis, x_acceleration);
        y_axis = DecreaseAxis(y_goal, y_axis, y_acceleration);
        z_axis = DecreaseAxis(z_goal, z_axis, z_acceleration, true);
        zx_axis = DecreaseAxis(zx_goal, zx_axis, z_acceleration, true);

        SetFollowObject();
        //GetPlayerOffset(0, 8, 0.5f, false);

        //dog position
        current_position = dog.transform.position;

        //Behaviour
        if (behav_switch.IsBehaviourON(3))
        {//aggressive
            Debug.Log("Be AGGRO");
            target_pos = agg_position.transform.position;
            agg_behav.dist_to_target = Vector3.Distance(dog.transform.position, agg_position.transform.position);
            agg_behav.dist_to_player = Vector3.Distance(dog.transform.position, player.transform.position);
            agg_behav.StopAgression();

            agg_behav.MoveToPositionAndFacePlayer();
        }
        else if (behav_switch.IsBehaviourON(2))
        {//friendly
            Debug.Log("Be Friendly");
            //maybe in own update
            player_interaction.IsCloseToLeftHand();
            player_interaction.IsCloseToRightHand();
            player_interaction.AreHandsMoving();

            if (y_goal == trot_value)
            {
                y_goal = walking_slow_value;
            }
            //TODO uncomment
            friendly_behav.ApproachPlayer();
        }
        else if (behav_switch.IsBehaviourON(0))
        {//pause
            pause_behav.CalculatePauseDist();
            if (pause_behav.go_to_location)
            {
                Debug.Log("Mache PAUSE");
                pause_behav.GoToPauseLocation();
            }
            //DodgePlayer(5);
        }
        else if (behav_switch.IsBehaviourON(4))
        {//chill
            Debug.Log("CHILL BOI");
            chill_behav.BeChill();
        }
        else if (behav_switch.IsBehaviourON(1))
        {//neutral
            neutral_behav.DoTrot();//TODO uncomment
            if (dog_state == Animation_state.walking && (neutral_behav.current_step == Neutral_Behaviour.Step.initial || neutral_behav.current_step == Neutral_Behaviour.Step.Stop))
            {
                if (MU.DodgePlayer(player, 3)) //TODO enable
                {
                    set_bbt_values(false, bbt_no_standing_value);
                    Debug.Log("DODgING IN NEUTRAL");
                    //MU.change_blend_tree_if_necessary(false);
                }
            }
        }

        //Change Animation on Timer depending on Behaviour
        if (change_anim_timer <= 0)
        {
            ChooseRandomIndex();
            turning_behav.TurningBehaviour();
            //behaviours
            if (behav_switch.IsBehaviourON(1) && !turning_behav.walking_after_turning_on)
            {//neutral
                neutral_behav.NeutralBehaviour();
            }
            else if (behav_switch.IsBehaviourON(2))
            {//friendly
                //TODO uncomment
                friendly_behav.FriendlyBehaviour();
                //GetPlayerOffset(0, 1, 1, true);
            }
            else if (behav_switch.IsBehaviourON(3))
            {//aggresive
                agg_behav.AggressiveBehaviour();
            }
            else if (behav_switch.IsBehaviourON(0))
            {//pause
                pause_behav.PauseBehaviour();
            }
            else if (behav_switch.IsBehaviourON(4))
            {
                chill_behav.ChillBehaviour();
            }
            ResetTimerFunction();
            //Debug.Log("new state " + dog_state);
        }
    }

}
