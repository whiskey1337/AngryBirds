using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayClip(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }

    public void PlayRandomClip(AudioClip[] clips, AudioSource source)
    {
        int randomIndex = Random.Range(0, clips.Length);

        source.clip = clips[randomIndex];
        source.Play();
    }
}