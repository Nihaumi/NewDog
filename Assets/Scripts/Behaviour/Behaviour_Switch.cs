using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Switch : MonoBehaviour
{
    //get scripts
    GameObject dog;
    GameObject behaviour_manager;
    public Friendly_Behaviour friendly_script;
    public Neutral_Behaviour neutral_script;
    public Basic_Behaviour basic_script;
    public Aggressive_Behaviour aggressive_script;
    public Pause_Behaviour pause_behav;
    Turning_Behaviour turning_behav;


    //get Player
    GameObject player;

    //distance to trigger behaviour
    public float dist;
    public float friendly_distance = 0.5f;

    [SerializeField] bool friendly;
    [SerializeField] bool agressive;
    [SerializeField] bool neutral;
    [SerializeField] bool paused;
    [SerializeField] float friendly_timer;
    [SerializeField] float agressive_timer;
    [SerializeField] float neutral_timer;
    [SerializeField] float pause_timer;
    [SerializeField] float friendly_time = 60f;
    [SerializeField] float agressive_time = 60f;
    [SerializeField] float neutral_time = 30f;
    [SerializeField] float pause_time = 30f;
    [SerializeField] float initial_pause_time = 20f;

    [SerializeField] bool friendly_on = false;
    [SerializeField] bool turning_on = false;
    [SerializeField] bool aggressive_on = false;
    [SerializeField] bool neutral_on = false;
    [SerializeField] bool paused_on = false;

    public bool enter_pause = false;
    //behaviour states
    public enum Behaviour_state
    {
        initial,
        friendly,
        neutral,
        aggressive,
        paused
    }
    public Behaviour_state dog_behaviour;


    // Start is called before the first frame update
    void Start()
    {
        //get scripts 
        dog = GameObject.Find("GermanShepherd_Prefab");
        behaviour_manager = GameObject.Find("Behaviour_Manager");
        friendly_script = dog.GetComponent<Friendly_Behaviour>();
        neutral_script = dog.GetComponent<Neutral_Behaviour>();
        basic_script = dog.GetComponent<Basic_Behaviour>();
        aggressive_script = dog.GetComponent<Aggressive_Behaviour>();
        turning_behav = dog.GetComponent<Turning_Behaviour>();
        pause_behav = dog.GetComponent<Pause_Behaviour>();

        //set scripts
        DisableScripts();
        //neutral = true;

        //player
        player = GameObject.FindGameObjectWithTag("Player");

        // is updated immediately on first call to Up
        dog_behaviour = Behaviour_state.initial;

        visited_behaviours_count = 0;

        ResetTimers();
        pause_timer = initial_pause_time;
    }

    float GetDistanceToObject(GameObject obj_1, GameObject obj_2)
    {
        dist = Vector3.Distance(obj_1.transform.position, obj_2.transform.position);
        return dist;
    }
    void ResetTimers()
    {
        friendly_timer = friendly_time;
        neutral_timer = neutral_time;
        agressive_timer = agressive_time;
        pause_timer = pause_time;
    }

    // Update is called once per frame
    void Update()
    {
        //if there is a plyer obj, check distance between player and dog and set scripts
        if (player)
        {
            GetDistanceToObject(player, dog);
            //CheckDistance();
        }
        if (friendly)
        {
            Debug.Log("FRIEND ON");
            DisableScripts();
            turning_on = false;
            friendly_on = true;
            friendly_timer = friendly_timer - Time.deltaTime * 1;
        }
        else if (neutral)
        {
            Debug.Log("NEUTRAL ON");
            DisableScripts();
            turning_on = true;
            neutral_on = true;
            neutral_timer = neutral_timer - Time.deltaTime * 1;
        }
        else if (agressive)
        {
            DisableScripts();
            turning_on = false;
            aggressive_on = true;
            agressive_timer = agressive_timer - Time.deltaTime * 1;
        }
        else if (paused)
        {
            Debug.Log("PAUSED ON");
            DisableScripts();
            turning_on = false;
            paused_on = true;
            pause_timer = pause_timer - Time.deltaTime * 1;
        }
        ChangeBehaviours();
    }

    [SerializeField] int visited_behaviours_count;

    void ChangeBehaviours()
    {
        if (/*friendly_timer <= 0 || */enter_pause)
        {
            Debug.Log("FRINED DOWN");
            SetScriptsFalse();
            DisableScripts();
            ResetTimers();
            visited_behaviours_count++;
            paused = true;
            enter_pause = false;
        }
        if (neutral_timer <= 0)
        {
            Debug.Log("NEUTRAL DOWN");
            SetScriptsFalse();
            DisableScripts();
            ResetTimers();
            visited_behaviours_count++;
            if (visited_behaviours_count == 1 % 5)
            {
                friendly = true;
            }
            else if (visited_behaviours_count == 3 % 5)
            {
                pause_time = 10f;
                paused = true;
            }
            else paused = true;

        }
        if (/*agressive_timer <= 0 || */enter_pause)
        {
            SetScriptsFalse();
            DisableScripts();
            ResetTimers();
            visited_behaviours_count++;
            paused = true;
            enter_pause = false;
        }
        if (pause_behav.end_pause)
        {
            Debug.Log("PAUSED DOWN");
            pause_behav.ResetBools();
            SetScriptsFalse();
            DisableScripts();
            ResetTimers();
            if (visited_behaviours_count == 0 % 5)
            {
                neutral = true;
            }
            else if (visited_behaviours_count == 2 % 5)
            {
                neutral = true;
                neutral_timer = 20f;
            }
            else if (visited_behaviours_count == 3 % 5)
            {
                agressive = true;
            }
            else if (visited_behaviours_count == 4 % 5)
            {
                neutral = true;
            }
            pause_behav.end_pause = false;
        }
    }

    void SetScriptsFalse()
    {
        agressive = false;
        neutral = false;
        friendly = false;
        paused = false;
    }

    public void DisableScripts()
    {
        friendly_on = false;
        aggressive_on = false;
        paused_on = false;
        neutral_on = false;
    }

    public bool IsBehaviourON(int behaviour_number)
    {
        bool is_behav_true = false;
        switch (behaviour_number)
        {
            case 0:
                if (paused_on == true)
                {
                    is_behav_true = true;
                }
                else is_behav_true = false;
                break;
            case 1:
                if (neutral_on == true)
                {
                    return true;
                }
                else is_behav_true = false;
                break;
            case 2:
                if (friendly_on == true)
                {
                    return true;
                }
                else is_behav_true = false;
                break;
            case 3:
                if (aggressive_on == true)
                {
                    return true;
                }
                else is_behav_true = false;
                break;
        }
        return is_behav_true;
    }
}
