using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controll : MonoBehaviour
{
    //animator
    public Animator animator;
    public string current_state;
    public string new_state;

    //animation time
    public float animation_duration;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //play new animation (if its not already playing)
    public void ChangeAnimationState(string new_state)
    {
        //Debug.Log("currenrt state: " + current_state + ", new state: " + new_state );
        //if same animation already plays, dont play it again
        if (current_state == new_state)
        {
            //Debug.Log("same anim as before " + current_state + "=" + new_state);
            return;
        }
        //play wanted animation
        animator.Play(new_state);
        //Debug.Log("play state: " + new_state);
        //animation_duration = new_state.Length;

        //set current state
        current_state = new_state;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
