using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TalkData
{
    public int QuestIndex;
    public int ObjectId;
    public List<DialogData> DialogData;
    public int SelectionKey;
    public int AcquiredItemKey;
    public int DisplayEnableItemKey;
    public int EndingKey;
    public int EffectSoundKey;

    public TalkData(int QuestIndex, int ObjectId, List<DialogData> DialogData, int selectionKey, int acquiredItemKey, int displayEnableItemKey, int endingKey, int effectSoundKey)
    {
        this.QuestIndex = QuestIndex;
        this.ObjectId = ObjectId;
        this.DialogData = DialogData;
        this.SelectionKey = selectionKey;
        this.AcquiredItemKey = acquiredItemKey;
        this.DisplayEnableItemKey = displayEnableItemKey;
        this.EndingKey = endingKey;
        this.EffectSoundKey = effectSoundKey;
    }
}

