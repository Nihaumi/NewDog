using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Sources : MonoBehaviour
{
    public AudioSource aggressive_bark;
    public AudioSource panting_calm;
    public AudioSource panting;
    public AudioSource bite_bark;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void StopAllSounds()
    {   
        aggressive_bark.Stop();
        panting.Stop();
        panting_calm.Stop();
        bite_bark.Stop();
    }

    public IEnumerator PlaySoundAfterPause(AudioSource audio)
    {
        StopAllSounds();
        yield return new WaitForSeconds(3);
        audio.Play();
    }
    public IEnumerator PlaySoundAfterAnother(AudioSource audio1, AudioSource audio2)
    {
        StopAllSounds();
        audio1.Play();
        yield return new WaitForSeconds(1f);
        StopAllSounds();
        audio2.Play();
    }
}
