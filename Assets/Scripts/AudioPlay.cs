using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{

    [SerializeField] AudioSource bark_audio_1;
    [SerializeField] AudioSource bark_audio_2;
    [SerializeField] AudioSource bark_audio_3;
    GameObject barker_1;
    GameObject barker_2;
    GameObject barker_3;
    // Start is called before the first frame update
    void Start()
    {
        barker_1 = GameObject.Find("barking (1)");
        barker_3 = GameObject.Find("barking (2)");
        barker_2 = GameObject.Find("barking (3)");
        bark_audio_1 = barker_1.GetComponent<AudioSource>();
        bark_audio_2 = barker_2.GetComponent<AudioSource>();
        bark_audio_3 = barker_3.GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        //BarkNow();
    }

    void BorkNow()
    {

    }
    void Bark(int bark)
    {
        Debug.Log("AUDIO: " + bark_audio_1);
        Debug.Log("MANGER: " + barker_1);
        Debug.Log("BORK BORK");
        switch (bark)
        {

            case 0:
                bark_audio_1.Play();
                break;
            case 1:
                bark_audio_2.Play();
                break;
            case 2:
                bark_audio_3.Play();
                break;
        }

    }
}
