using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SoundEvent
{
    public string SoundKey;
    public AudioClip AudioSource;
}

[Serializable]
public class SceneBGMData
{
    public int SceneIndex;
    public string BGMSoundKey;
}

public class SoundManager : MonoBehaviour
{
    public List<SoundEvent> SFXEventList;
    public List<SoundEvent> BGMEventList;
    public List<SceneBGMData> SceneBGMData;
    public AudioSource SFXAudioSource;
    public AudioSource BGMAudioSource;

    public AudioClip GetSFXAudioClip(string targetSoundKey)
    {
        if(targetSoundKey == "")
        {
            return null;
        }

        foreach(SoundEvent sound in SFXEventList)
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
        AudioClip audioClip = GetSFXAudioClip(targetSoundKey);
        if(audioClip != null && SFXAudioSource != null)
        {
            SFXAudioSource.clip = audioClip;
            SFXAudioSource.Play();
            return true;
        }
        return false;
    }

    public bool PlayBGMLoopBySoundKey(string InSoundKey)
    {
        foreach (SoundEvent InBGMEVent in BGMEventList)
        {
            if(InBGMEVent.SoundKey == InSoundKey && InBGMEVent.AudioSource != null && BGMAudioSource != null)
            {
                BGMAudioSource.clip = InBGMEVent.AudioSource;
                BGMAudioSource.Play();
                return true;
            }
        }

        return false;
    }

    public bool PlayBGMLoopBySceneIndex(int InSceneIndex)
    {
        foreach (SceneBGMData BGMData in SceneBGMData)
        {
            if (BGMData.SceneIndex == InSceneIndex)
            {
                return PlayBGMLoopBySoundKey(BGMData.BGMSoundKey);
            }
        }

        return false;
    }

    public void StopBGM()
    {
        if(BGMAudioSource != null)
        {
            BGMAudioSource.Stop();
        }          
    }
}
