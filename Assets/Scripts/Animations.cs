using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    //animations grouped as they are in the animator 


    //friendly
    public string friendly_blend_tree = "friendly_tree";
    public string friendly_trans_sleep_to_stand = "Trans_Sleeping_to_Lying_to_Stand_02_friendly";
    public string friendly_trans_sleep_to_lying = " Trans_Sleeping_to_Lying_friendly";
    public string friendly_trans_lying_to_stand = "Trans_Lying_to_Stand_02_friendly";
    public string friendly_trans_sitting_to_stand = "Trans_Sitting_to_Stand_02_friendly";
    public string friendly_trans_stand_to_sitting = "Trans_Stand_to_Sitting_00 1";
    public string friendly_trans_stand_to_lying = "Trans_Stand_to_Lying_00";
    public string friendly_turn_after_sitting = "friendly_turn_after_touching";
    public string friendly_lying = "Lying_00";
    public string friendly_stand = "Stand_00";
    public string friendly_sitting = "Sitting_00";
    public string friendly_sit_to_turn_walk = "friendly_sit_to_turn_to_walk";

    //aggressive
    public string aggresive_blend_tree = "Aggressive_BT";
    public string trans_agg_to_stand = "Trans_Agressive_to_Stand";
    public string trans_stand_to_agg = "Trans_Stand_to_Agressive";
    public string stand_agg = "Stand_02A";
    public string aggressive = "Agressive_01";
    public string bite_L = "Attack_BiteL_Long";
    public string bite_R = "Attack_BiteR_Long";
    public string agg_trans_sleep_to_stand = "agg_trans_sleep_to_stand";
    public string agg_trans_lying_to_stand = "agg_trans_lying_to_stand";
    public string agg_trans_sit_to_stand = "agg_trans_sit_to_stand";


    //neutral

    //blending trees
    public string blend_tree = "Blend_Tree";
    public string turn_walk_slow_tree = "Blend_Tree_Turn_Walk_Slow";
    public string turn_walk_tree = "Blend_Tree_Turn_Walk";
    public string turn_seek_tree = "Blend_Tree_Turn_Seek";
    public string turn_trot_tree = "Blend_Tree_Turn_Trot";
    public string blend_tree_seek = "Blend_Tree_Seeking";
    public string blend_tree_MU = "MU no standing";
    public string bbt = "Blend_blend_tree";
    public string all_walks_bt = "All Walking BT";
    public string seek_bt = "Seek BT";


    //transition x to standing in bleding BT
    public string bbt_trans_sleep_to_stand = "trans_sleep_to_stand_BBT";
    public string bbt_trans_lying_to_stand = "trans_lying_to_stand_BBT";
    public string bbt_trans_sit_to_stand = "trans_sit_to_stand_BBT";
    public string bbt_trans_aggro_to_stand = "Trans_Agressive_to_Stand_BBT";

    //standing
    public string stand_00 = "Stand_00";
    public string stand_01 = "Stand_01";
    public string trans_lying_to_stand_01 = "Trans_Lying_to_Stand_01";
    public string trans_sitting_to_stand_agg = "Trans_Sitting_to_Stand_agg";
    //public string trans_sitting_to_stand_01 = "Trans_Lying_to_Stand_01";
    public string trans_sleeping_to_lying_to_stand_02 = "Trans_Sleeping_to_Lying_to_Stand_02";
    public string stand_02 = "Stand_02";
    public string trans_lying_to_stand_02 = "Trans_Lying_to_Stand_02";
    public string trans_sitting_to_stand_02 = "Trans_Sitting_to_Stand_02";

    //sitting
    public string sit_00 = "Sitting_00";
    public string trans_stand_to_sit_00 = "Trans_Stand_to_Sitting_00";
    public string trans_walk_to_stand_to_sit_00 = "Trans_Walk_to_Stand_to_Sitting_00";
    public string sit_01 = "Sitting_01";
    public string trans_stand_to_sit_01 = "Trans_Stand_to_Sitting_01";
    public string trans_walk_to_stand_to_sit_01 = "Trans_Walk_to_Stand_to_Sitting_01";
    public string sit_02 = "Sitting_02";
    public string trans_stand_to_sit_02 = "Trans_Stand_to_Sitting_02";
    public string trans_walk_to_stand_to_sit_02 = "Trans_Walk_to_Stand_to_Sitting_02";
    //lying
    public string lying_00 = "Lying_00";
    public string trans_stand_to_lying_00 = "Trans_Stand_to_Lying_00";
    public string trans_walk_to_stand_to_lying_00 = "Trans_Walk_to_Stand_to_Lying_00";
    public string trans_sleep_to_lying = "Trans_Sleeping_to_Lying_00";
    public string lying_01 = "Lying_01";
    public string trans_stand_to_lying_01 = "Trans_Stand_to_Lying_01";
    public string trans_walk_to_stand_to_lying_01 = "Trans_Walk_to_Stand_to_Lying_00";
    public string lying_02 = "Lying_02";
    public string trans_stand_to_lying_02 = "Trans_Stand_to_Lying_02";
    public string trans_walk_to_stand_to_lying_02 = "Trans_Walk_to_Stand_to_Lying_00";
    //sleeping
    public string sleep = "Sleeping_01";
    public string trans_lying_to_sleep = "Trans_Lying_to_Sleeping_01";
    public string trans_stand_to_lying_to_sleep = "Trans_Stand_to_Lying__Sleeping_01";
    public string trans_walk_to_stand_to_lying_to_sleep = "Trans_Walk_to_Stand_to_Lying_to_Sleeping_01";
    //slow walk
    public string walk_slow = "Loco_WalkSlow_Copy";
    public string trans_sit_to_stand_to_walk_slow = "Trans_Sitting_to_Stand_plus_WalkSlow";
    public string trans_lying_to_stand_to_walk_slow = "Trans_Lying_to_Stand_plus_WalkSlow";
    public string trans_sleep_to_lying_to_stand_to_walk_slow = "Trans_Sleeping_to_Lying_to_Stand_plus_WalkSlow";
    public string walk_slow_L = "Loco_WalkSlow_L";
    public string walk_slow_R = "Loco_WalkSlow_R";
    //seek walk
    public string seek = "Loco_WalkSeek";
    public string trans_sit_to_stand_to_seek = "Trans_Sitting_to_Stand_plus_seek";
    public string trans_lying_to_stand_to_seek = "Trans_Lying_to_Stand_plus_seek";
    public string trans_sleep_to_lying_to_stand_to_seek = "Trans_Sleeping_to_Lying_to_Stand_plus_seek";
    public string seek_L = "Loco_WalkSlow_Seek_L";
    public string seek_R = "Loco_WalkSlow_Seek_R";
    public string turn_left_seek = "Trans_TurnL90_Seek";
    public string turn_right_seek = "Trans_TurnR90_Seek";
    //normal walk
    public string walk = "Loco_Walk";
    public string trans_sit_to_stand_to_walk = "Trans_Sitting_to_Stand_plus_Walk";
    public string trans_lying_to_stand_to_walk = "Trans_Lying_to_Stand_plus_Walk";
    public string trans_sleep_to_lying_to_stand_to_walk = "Trans_Sleeping_to_Lying_to_Stand_plus_Walk";
    public string walk_L = "Loco_Walk_L";
    public string walk_R = "Loco_Walk_R";
    public string turn_left_walk = "Trans_TurnL90_Walk";
    public string turn_right_walk = "Trans_TurnR90_Walk";
    public string walk_back = "Loco_WalkBack";
    //trot
    public string trot = "Loco_Trot";
    public string trans_sit_to_stand_to_trot = "Trans_Sitting_to_Stand_plus_Trot";
    public string trans_lying_to_stand_to_trot = "Trans_Lying_to_Stand_plus_Trot";
    public string trans_sleep_to_lying_to_stand_to_trot = "Trans_Sleeping_to_Lying_to_Stand_plus_Trot";
    public string trot_L = "Loco_Trot_L";
    public string trot_R = "Loco_Trot_R";
    public string turn_left_trot = "Trans_TurnL90_Trot";
    public string turn_right_trot = "Trans_TurnR90_Trot";
    //run
    public string run = "Loco_Run";
    public string trans_sit_to_stand_to_run = "Trans_Sitting_to_Stand_plus_Run";
    public string trans_lying_to_stand_to_run = "Trans_Lying_to_Stand_plus_Run";
    public string trans_sleep_to_lying_to_stand_to_run = "Trans_Sleeping_to_Lying_to_Stand_plus_Run";
    public string run_L = "Loco_Run_L";
    public string run_R = "Loco_Run_R";

    //turn
    public string turn_left_90_deg_L = "Trans_TurnL90";
    public string turn_left_90_deg_R = "Trans_TurnR90";

    //animation lists - to pick a random animation
    public List<string> list_standing = new List<string>();

    public List<string> list_sitting = new List<string>();

    public List<string> list_lying = new List<string>();

    public List<string> list_walking = new List<string>();

    public List<string> list_running = new List<string>();

    public List<string> list_sleeping = new List<string>();

    public List<string> list_walking_after_turning = new List<string>();

    //friendly

    public List<string> friendly_list_sitting = new List<string>();

    public List<string> friendly_list_lying = new List<string>();

    public List<string> friendly_list_sleeping = new List<string>();


    public List<string> friendly_list_standing = new List<string>();

    public List<string> friendly_list_walking = new List<string>();

    //aggressive
    public List<string> agg_list = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        //agg list
        agg_list.Add("0");
        agg_list.Add("1");
        agg_list.Add("2");

        //fill friendly list
        //siting
        friendly_list_sitting.Add(friendly_trans_sitting_to_stand);//TODO bbt
        friendly_list_sitting.Add(walk_slow);
        friendly_list_sitting.Add(walk);

        //lying
        friendly_list_lying.Add(trans_lying_to_sleep);
        friendly_list_lying.Add(bbt_trans_lying_to_stand);
        friendly_list_lying.Add(walk_slow);//walkSlow
        friendly_list_lying.Add(walk);//walk

        //sleeping
        friendly_list_sleeping.Add(friendly_trans_sleep_to_lying);
        friendly_list_sleeping.Add(bbt_trans_sleep_to_stand);
        friendly_list_sleeping.Add(walk_slow);//walkSlow
        friendly_list_sleeping.Add(walk);//walk

        //standing
        friendly_list_standing.Add(friendly_trans_stand_to_lying);
        friendly_list_standing.Add(friendly_trans_stand_to_sitting);
        friendly_list_standing.Add(walk_slow);//walkSlow
        friendly_list_standing.Add(walk);//walk


        //walking
        friendly_list_walking.Add(stand_00);
        friendly_list_walking.Add(walk_slow);//walkSlow
        friendly_list_walking.Add(walk);//walk


        //fill lists neutral
        //stand
        list_standing.Add(trans_stand_to_lying_00);//0
        list_standing.Add(trans_stand_to_sit_00);//1
        list_standing.Add(walk_slow);//2
        list_standing.Add(walk); //3
        list_standing.Add(seek);//4
        list_standing.Add(trot);//5
                                //list_standing.Add(turn_left_seek);//6
                                //list_standing.Add(turn_right_seek);//7
                                //list_standing.Add(run);

        //sit
        list_sitting.Add(trans_sit_to_stand_to_walk_slow);
        list_sitting.Add(trans_sit_to_stand_to_walk);
        list_sitting.Add(trans_sit_to_stand_to_seek);
        list_sitting.Add(trans_sit_to_stand_to_trot);
        //list_sitting.Add(trans_sit_to_stand_to_run);

        //run
        list_running.Add(stand_02);
        list_running.Add(trans_lying_to_stand_to_seek);
        list_running.Add(trans_lying_to_stand_to_trot);
        list_running.Add(trans_lying_to_stand_to_walk);
        list_running.Add(trans_lying_to_stand_to_walk_slow);

        //lying
        list_lying.Add(trans_lying_to_sleep);
        list_lying.Add(trans_lying_to_stand_to_walk_slow);
        list_lying.Add(trans_lying_to_stand_to_walk);
        list_lying.Add(trans_lying_to_stand_to_seek);
        list_lying.Add(trans_lying_to_stand_to_trot);
        //list_lying.Add(trans_lying_to_stand_to_run);

        //sleep
        list_sleeping.Add(trans_sleep_to_lying);
        list_sleeping.Add(bbt_trans_sleep_to_stand);
        list_sleeping.Add(bbt_trans_sleep_to_stand);
        list_sleeping.Add(bbt_trans_sleep_to_stand);
        list_sleeping.Add(bbt_trans_sleep_to_stand);
        list_sleeping.Add(bbt_trans_sleep_to_stand);
        //list_sleeping.Add(trans_sleep_to_lying_to_stand_to_run);


        //walk
        list_walking.Add(bbt);//0
        list_walking.Add(walk_slow);//1
        list_walking.Add(walk);//2
        list_walking.Add(seek);//3
        list_walking.Add(trot);//4
                               //list_walking.Add(run);//5
                               //list_walking.Add(seek_L);//6
                               //list_walking.Add(seek_R);//7

        //after turning
        list_walking_after_turning.Add(seek);
        list_walking_after_turning.Add(walk);
        list_walking_after_turning.Add(walk_slow);
        list_walking_after_turning.Add(trot);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
