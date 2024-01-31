using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

[Serializable]
public class CutSceneData
{
    public string CutSceneKey;
    public Sprite CutSceneSprite;
}


public class UIManager : MonoBehaviour
{
    public GameObject cutSceneUI;
    public List<CutSceneData> CutSceneDataList;

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

    public void ShowCutScene(string InCutSceneKey)
    {
        if(cutSceneUI != null)
        {
            cutSceneUI.SetActive(true);

            Image CutSceneImage = cutSceneUI.GetComponentInChildren<Image>();
            if (CutSceneImage != null)
            {
                foreach (CutSceneData cutSceneData in CutSceneDataList)
                {
                    if (cutSceneData.CutSceneKey == InCutSceneKey)
                    {
                        CutSceneImage.sprite = cutSceneData.CutSceneSprite;
                        break;
                    }
                }
            }
        }
    }

    public void HideCutScene()
    {
        if(cutSceneUI != null)
        {
            cutSceneUI.SetActive(false);
        }
    }
}
