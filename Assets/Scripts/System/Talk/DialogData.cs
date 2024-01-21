using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogData
{
    public string DialogText;
    public float TalkDelay;
    public int PortraitNum;
    public string CutSceneKey;
    public string AnimationKey;
    public string EffectSoundKey;

    public DialogData(string DialogText, float TalkDelay, int PortraitNum, string CutsceneKey, string AnimationKey, string EffectSoundKey)
    {
        this.DialogText = DialogText;
        this.TalkDelay = TalkDelay;
        this.PortraitNum = PortraitNum;
        this.CutSceneKey = CutsceneKey;
        this.AnimationKey = AnimationKey;
        this.EffectSoundKey = EffectSoundKey;
    }
}
