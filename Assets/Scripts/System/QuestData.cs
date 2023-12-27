using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[Serializable]
public class QuestData
{
    public int questId;
    public string questName;
    public int[] objectId;

    public QuestData(int questId, string questName, int[] objectId)
    {
        this.questId = questId;
        this.questName = questName;
        this.objectId = objectId;
    }
}
