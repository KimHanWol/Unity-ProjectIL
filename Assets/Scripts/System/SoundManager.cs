using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEvent
{
    public string SoundKey;
    public AudioClip AudioSource;
}


public class SoundManager : MonoBehaviour
{
    public List<SoundEvent> SoundEventList;
    public AudioSource audioSource;

    public AudioClip GetAudioClip(string targetSoundKey)
    {
        if(targetSoundKey == "")
        {
            return null;
        }

        foreach(SoundEvent sound in SoundEventList)
        {
            if(sound.SoundKey == targetSoundKey)
            {
                return sound.AudioSource;
            }
        }

        Debug.Log("There's no " + targetSoundKey + " in SoundManager");
        return null;
    }

    public bool PlayAudioSound(string targetSoundKey)
    {
        AudioClip audioClip = GetAudioClip(targetSoundKey);
        if(audioClip != null && audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            return true;
        }
        return false;
    }
}
