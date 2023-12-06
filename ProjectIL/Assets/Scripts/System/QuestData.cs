using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

public class QuestData
{
    public string questName;
    public int[] npcId;

    public QuestData(string name, int[] npc)
    {
        questName = name;
        npcId = npc;
    }
}
