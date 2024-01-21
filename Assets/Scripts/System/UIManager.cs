using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum AnimationType
{ 
    FadeIn,
    FadeOut,
    FadeInOut,
}

[Serializable]
public class AnimationData
{ 
    public AnimationType AnimationType;
    public Animator TargetAnimator;
}

public class UIManager : MonoBehaviour
{
    public List<AnimationData> AnimationList;

    public float PlayAnimation(string AnimationKeyString)
    {
        if(AnimationKeyString == null || AnimationKeyString == "")
        {
            return 0;
        }

        AnimationType TargetAnimation = (AnimationType)Enum.Parse(typeof(AnimationType), AnimationKeyString);

        foreach(AnimationData data in AnimationList)
        {
            if(data.AnimationType == TargetAnimation)
            {
                data.TargetAnimator.SetTrigger(AnimationKeyString);
                return data.TargetAnimator.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        Debug.Log("Can't Play Animation Because Can't Find Animation (" + AnimationKeyString + ")");
        return 0;
    }


}
