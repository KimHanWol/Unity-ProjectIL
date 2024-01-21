using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TalkData
{
    public int QuestIndex;
    public int QuestActionIndex;
    public int ObjectId;
    public List<DialogData> DialogData;

    public TalkData(int QuestIndex, int QuestActionIndex, int ObjectId, List<DialogData> DialogData, string CutSceneKey, string AnimationKey, string EffectSoundKey)
    {
        this.QuestIndex = QuestIndex;
        this.QuestActionIndex = QuestActionIndex;
        this.ObjectId = ObjectId;
        this.DialogData = DialogData;
    }
}

