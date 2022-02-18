using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Detection : MonoBehaviour
{
    public bool collided;
    public bool hit_corner;

    public bool left_trot_touching = false;
    public bool right_trot_touching = false;
    public bool trot_collider_touching_wall = false;

    //cube that collided
    public GameObject side;

    //objects
    GameObject dir_manager;
    GameObject dog;

    //scripts
    Turning_Direction_Handler turning_dir_handler;
    Basic_Behaviour basic_behav;

    // Start is called before the first frame update
    void Start()
    {
        //obj & scripts
        dog = GameObject.Find("GermanShepherd_Prefab");
        dir_manager = GameObject.Find("Direction_Manager");
        turning_dir_handler = dir_manager.GetComponent<Turning_Direction_Handler>();
        basic_behav = dog.GetComponent<Basic_Behaviour>();

        collided = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Corner")
        {
            if (gameObject.name == "left_trot")
            {
                left_trot_touching = true;
            }
            if(gameObject.name == "right_trot")
            {
                right_trot_touching = true;
            }

            if (gameObject.name == "left_trot" || gameObject.name == "right_trot")
            {
                trot_collider_touching_wall = true;
                if (basic_behav.y_goal == Basic_Behaviour.trot_value)
                {

                    collided = true;
                    GetCollidedObject(this.gameObject);
                    Debug.Log("TROT COLLISION with cube: " + side.name);
                    if (collision.gameObject.tag == "Corner")
                    {
                        hit_corner = true;
                    }
                }
            }
            else if (basic_behav.y_goal != Basic_Behaviour.trot_value && !collided)
            {
                collided = true;
                GetCollidedObject(this.gameObject);
                Debug.Log("COLLISION with cube: " + side.name);
                if (collision.gameObject.tag == "Corner")
                {
                    hit_corner = true;
                }
            }
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Environment" || collision.gameObject.tag == "Corner")
        {
            CheckIfTrotsAreTouchingWalls();

            Debug.Log("EXIT collision with: " + collision.gameObject.name);
            collided = false;
            hit_corner = false;
            turning_dir_handler.turn_90_deg = false;
        }
    }

    void CheckIfTrotsAreTouchingWalls()
    {
        if (gameObject.name == "left_trot")
        {
            left_trot_touching = false;
        }
        if (gameObject.name == "right_trot")
        {
            right_trot_touching = false;
        }
        if (!left_trot_touching && !right_trot_touching)
        {
            trot_collider_touching_wall = false;
        }
    }

    public void GetCollidedObject(GameObject cube)
    {
        side = cube;
    }
}
