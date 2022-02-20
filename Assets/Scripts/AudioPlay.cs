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
    [SerializeField] AudioSource[] all_but_panting;

    [SerializeField] AudioSource current_audio_source = null;

    //volume for fade
    float start_vol = 0.00f;
    float start_vol_pant;
    float start_vol_sniff;

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
        all_sounds = new AudioSource[] { pant_audio, slow_pant_audio, jump_bark_audio, sniff_audio };
        all_but_panting = new AudioSource[] { jump_bark_audio, sniff_audio };
        step_sounds = new AudioSource[] { step_4_audio, step_3_audio, step_2_audio, step_1_audio };

        //start_vol_pant = 0.3f;
        //start_vol_sniff = 0.4f;
    }

    void SetCurrentAudioSource(AudioSource audio_source)
    {
        current_audio_source = audio_source;
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

    bool fade = false;
    float fade_time = 0.5f;
    // Update is called once per frame
    void Update()
    {

    }

    void FadeOut(AudioSource audio_source)
    {
        /*
                if (start_vol == 0.00f)
                {
                    if (audio_source == pant_audio)
                    {
                        start_vol = start_vol_pant;
                    }
                    else
                    {
                        start_vol = start_vol_sniff;
                    }
                }

                if (audio_source.isPlaying)
                {
                    if (audio_source.volume > 0.03f)
                    {
                        audio_source.volume -= start_vol * Time.deltaTime / fade_time;
                    }
                    else
                    {
                        audio_source.Stop();
                        ResetVolumes(audio_source);
                    }
                }
        */
    }

    void ResetVolumes(AudioSource audio_source)
    {

        if (audio_source == sniff_audio)
        {
            audio_source.volume = start_vol_sniff;
        }
    }

    void BarkNow()
    {
        int bark = GetRandomBarkIndex();
        audios[bark].Play();
        SetCurrentAudioSource(audios[bark]);
    }

    void JumpBark()
    {
        jump_bark_audio.Play();
        SetCurrentAudioSource(jump_bark_audio);
    }

    void Growl()
    {
        growl_audio.Play();
        SetCurrentAudioSource(growl_audio);
    }

    int last_step = 0;
    void Step()
    {
        /*int step = GetRandomStepIndex();
        Debug.Log("step = " + step);
        Debug.Log("last step = " + last_step);
        if(step == last_step)
        {
            return;
        }
        else
        {
            if (step_sounds[last_step].isPlaying)
            {
                step_sounds[last_step].Stop();
            }
            step_sounds[step].Play();
        }
        last_step = step;
         SetCurrentAudioSource(step_sounds[step]);
         */
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
                if (!pant_audio.isPlaying)
                {
                    pant_audio.Play();
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
        SetCurrentAudioSource(pant_audio);

    }

    void Sniff(int play)
    {
        switch (play)
        {
            case 0:
                if (sniff_audio.isPlaying)
                {
                    //FadeOut(sniff_audio);
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
        SetCurrentAudioSource(sniff_audio);
    }

    void StopSounds()
    {
        foreach (AudioSource sound in all_sounds)
        {
            sound.Stop();
        }
        SetCurrentAudioSource(null);
        //ResetVolumes(pant_audio);
        //ResetVolumes(sniff_audio);
    }

    void StopEverythingButPanting()
    {
        foreach (AudioSource sound in all_but_panting)
        {
            sound.Stop();
        }
        SetCurrentAudioSource(null);
    }
}
