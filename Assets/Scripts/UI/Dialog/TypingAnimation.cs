using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingAnimation : MonoBehaviour
{
    public int CharPerSeconds;
    public GameObject EndCursor;
    public bool bIsPlaying;

    public Text msgText;
    public AudioSource audioSource;

    string targetMsg;
    int index;
    float interval;

    public void SetMsg(string msg)
    {
        if(bIsPlaying)
        {
            msgText.text = targetMsg;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            targetMsg = msg;
            EffectStart();
        }
    }

    void EffectStart()
    {
        msgText = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();

        msgText.text = "";
        index = 0;
        EndCursor.SetActive(false);

        interval = 1.0f / CharPerSeconds;

        bIsPlaying = true;
        Invoke("Effecting", interval);
    }

    void Effecting()
    {
        if(msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        if(targetMsg == null)
        {
            EffectEnd();
            return;
        }

        msgText.text += targetMsg[index];
        //Sound
        if (targetMsg[index] != ' ' && targetMsg[index] != '.')
        {
            audioSource.Play();
        }

        index++;

        Invoke("Effecting", interval);
    }

    void EffectEnd()
    {
        bIsPlaying = false;
        EndCursor.SetActive(true);
    }

    public bool IsEffectOver()
    {
        return bIsPlaying == false && targetMsg != "" && msgText.text == targetMsg;
    }

}
