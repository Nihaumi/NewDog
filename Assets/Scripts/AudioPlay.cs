using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{

    [SerializeField] AudioSource bark_audio_1;
    [SerializeField] AudioSource bark_audio_2;
    [SerializeField] AudioSource bark_audio_3;
    [SerializeField] AudioSource bark_audio_5;
    [SerializeField] AudioSource bark_audio_6;
    [SerializeField] AudioSource bark_audio_7;
    [SerializeField] AudioSource bark_audio_8;
    [SerializeField] AudioSource pant_audio;
    [SerializeField] AudioSource slow_pant_audio;
    [SerializeField] AudioSource jump_bark_audio;
    [SerializeField] AudioSource sniff_audio;
    [SerializeField] AudioSource growl_audio;
    [SerializeField] AudioSource step_1_audio;
    [SerializeField] AudioSource step_2_audio;
    [SerializeField] AudioSource step_3_audio;
    [SerializeField] AudioSource step_4_audio;


    GameObject barker_1;
    GameObject barker_2;
    GameObject barker_3;
    GameObject barker_5;
    GameObject barker_6;
    GameObject barker_7;
    GameObject barker_8;
    GameObject panter;
    GameObject slow_panter;
    GameObject jump_bark;
    GameObject sniffer;
    GameObject growl;
    GameObject step_1;
    GameObject step_2;
    GameObject step_3;
    GameObject step_4;


    //list with Audio sources
    [SerializeField] AudioSource[] audios;
    [SerializeField] AudioSource[] all_sounds;
    [SerializeField] AudioSource[] step_sounds;

    // Start is called before the first frame update
    void Start()
    {
        barker_1 = GameObject.Find("barking (1)");
        barker_2 = GameObject.Find("barking (2)");
        barker_3 = GameObject.Find("barking (3)");
        barker_5 = GameObject.Find("barking (5)");
        barker_6 = GameObject.Find("barking (6)");
        barker_7 = GameObject.Find("barking (7)");
        barker_8 = GameObject.Find("barking (8)");
        panter = GameObject.Find("panting");
        sniffer = GameObject.Find("sniff");
        slow_panter = GameObject.Find("slow_pant");
        jump_bark = GameObject.Find("jump_bark");
        growl = GameObject.Find("growl");
        step_1 = GameObject.Find("step");
        step_2 = GameObject.Find("step (1)");
        step_3 = GameObject.Find("step (2)");
        step_4 = GameObject.Find("step (3)");

        bark_audio_1 = barker_1.GetComponent<AudioSource>();
        bark_audio_2 = barker_2.GetComponent<AudioSource>();
        bark_audio_3 = barker_3.GetComponent<AudioSource>();
        bark_audio_5 = barker_5.GetComponent<AudioSource>();
        bark_audio_6 = barker_6.GetComponent<AudioSource>();
        bark_audio_7 = barker_7.GetComponent<AudioSource>();
        bark_audio_8 = barker_8.GetComponent<AudioSource>();
        pant_audio = panter.GetComponent<AudioSource>();
        slow_pant_audio = slow_panter.GetComponent<AudioSource>();
        jump_bark_audio = jump_bark.GetComponent<AudioSource>();
        sniff_audio = sniffer.GetComponent<AudioSource>();
        growl_audio = growl.GetComponent<AudioSource>();
        step_1_audio = step_1.GetComponent<AudioSource>();
        step_2_audio = step_2.GetComponent<AudioSource>();
        step_3_audio = step_3.GetComponent<AudioSource>();
        step_4_audio = step_4.GetComponent<AudioSource>();

        audios = new AudioSource[] { bark_audio_1, bark_audio_2, bark_audio_3, bark_audio_5, bark_audio_6, bark_audio_7, bark_audio_8 };
        all_sounds = new AudioSource[] {pant_audio, slow_pant_audio, jump_bark_audio, sniff_audio };
        step_sounds = new AudioSource[] { step_4_audio, step_3_audio, step_2_audio, step_1_audio };
    }


    //choose random index
    private int GetRandomBarkIndex()
    {
        int index = Random.Range(0, audios.Length - 1);
        return index;
    }

    private int GetRandomStepIndex()
    {
        int index = Random.Range(0, step_sounds.Length - 1);
        return index;
    }

    // Update is called once per frame
    void Update()
    {
        //BarkNow();
    }

    void BarkNow()
    {
        int bark = GetRandomBarkIndex();
        Debug.Log("bark sound : " + audios[bark]);

        audios[bark].Play();

    }
    void JumpBark()
    {
        jump_bark_audio.Play();
    }

    void Growl()
    {
        growl_audio.Play();
    }

    void Step()
    {
        int step = GetRandomStepIndex();
        step_sounds[step].Play();
    }

    void Pant(int speed)
    {
        switch (speed)
        {
            case 1:
                if (!pant_audio.isPlaying)
                {
                    pant_audio.Play();
                }
                break;
            case 0:
                if (!slow_pant_audio.isPlaying)
                {
                slow_pant_audio.Play();
                }
                break;
            case -1:
                if (slow_pant_audio.isPlaying)
                {
                    slow_pant_audio.Stop();
                }
                else if (pant_audio.isPlaying)
                {
                    pant_audio.Stop();
                }
                break;
        }

    }

    void Sniff(int play)
    {
        switch (play)
        {
            case 0:
                if (sniff_audio.isPlaying)
                {
                    sniff_audio.Stop();
                }
                break;
            case 1:
                if (!sniff_audio.isPlaying)
                {
                    sniff_audio.Play();
                }
                break;
        }

    }

    void StopSounds()
    {
        foreach (AudioSource sound in all_sounds)
            sound.Stop();
    }


}
