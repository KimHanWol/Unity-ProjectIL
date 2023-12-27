using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogData
{
    public string DialogText;
    public int PortraitNum;
    public int CutsceneKey;
    public float TalkDelay;

    public DialogData(string dialogText, int portraitNum, int cutsceneKey, float talkDelay)
    {
        DialogText = dialogText;
        PortraitNum = portraitNum;
        CutsceneKey = cutsceneKey;
        TalkDelay = talkDelay;
    }
}
