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
    public GameObject menuUI;
    public GameObject DebugUI;
    public GameObject QuestUI;
    public GameObject PortraitUI;
    public GameObject DialogUI;

    public TypingAnimation typingAnimation;
    public Animator DialogAnimator;
    private Animator PortraitAnim;

    public List<CutSceneData> CutSceneDataList;

    public List<AnimationData> AnimationList;

    public float PlayAnimation(string AnimationKeyString)
    {
        if (AnimationKeyString == null || AnimationKeyString == "")
        {
            return 0;
        }

        AnimationType TargetAnimation = (AnimationType)Enum.Parse(typeof(AnimationType), AnimationKeyString);

        foreach (AnimationData data in AnimationList)
        {
            if (data.AnimationType == TargetAnimation)
            {
                if(data.TargetAnimator != null)
                {
                    data.TargetAnimator.SetTrigger(AnimationKeyString);
                    return data.TargetAnimator.GetCurrentAnimatorStateInfo(0).length;
                }
            }
        }

        Debug.Log("Can't Play Animation Because Can't Find Animation (" + AnimationKeyString + ")");
        return 0;
    }

    public void ShowCutScene(string InCutSceneKey)
    {
        if (cutSceneUI != null)
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
        if (cutSceneUI != null)
        {
            cutSceneUI.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        if (menuUI != null)
        {
            menuUI.SetActive(!menuUI.activeInHierarchy);
        }
    }

    public void ToggleDebugUI()
    {
        if (DebugUI != null)
        {
            DebugUI.SetActive(!DebugUI.activeInHierarchy);
        }
    }

    public void AddDebugText(string Intext)
    {
        if (DebugUI != null)
        {
            Text DebugText = DebugUI.GetComponent<Text>();
            if (DebugText != null)
            {
                DebugText.text += Intext + '\n';
            }
        }
    }

    public void SetQuestText(string InText)
    {
        if (QuestUI != null)
        {
            Text QuestText = QuestUI.GetComponentInChildren<Text>();
            if (QuestText != null)
            {
                QuestText.text = InText;
            }
        }
    }

    public bool IsTyping()
    {
        if (typingAnimation != null)
        {
            return typingAnimation.bIsPlaying;
        }
        return false;
    }

    public void SetTypingText(string InText)
    {
        if (typingAnimation != null)
        {
            typingAnimation.SetMsg(InText);
        }
    }

    public void SetPortraitColor(Color NewColor)
    {
        if(PortraitUI != null)
        {
            Image PortraitImage = PortraitUI.GetComponent<Image>();
            if(PortraitImage != null)
            {
                PortraitImage.color = NewColor;
            }
        }
    }

    public void EnableDialogUIWithAnimation(bool bEnable)
    {
        if(DialogAnimator != null)
        {
            DialogAnimator.SetBool("isShow", bEnable);
        }
    }
}
