using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TalkData
{
    public int QuestIndex;
    public int ObjectId;
    public List<Tuple<string, int>> DialogData;
    public float TalkDelay;
    public int SelectionKey;
    public int AcquiredItemKey;
    public int DisplayEnableItemKey;
    public int EndingKey;
    public int EffectSoundKey;

    public TalkData(int QuestIndex, int ObjectId, List<Tuple<string, int>> DialogData, float talkDelay, int selectionKey, int acquiredItemKey, int displayEnableItemKey, int endingKey, int effectSoundKey)
    {
        this.QuestIndex = QuestIndex;
        this.ObjectId = ObjectId;
        this.DialogData = DialogData;
        this.TalkDelay = talkDelay;
        this.SelectionKey = selectionKey;
        this.AcquiredItemKey = acquiredItemKey;
        this.DisplayEnableItemKey = displayEnableItemKey;
        this.EndingKey = endingKey;
        this.EffectSoundKey = effectSoundKey;
    }
}

