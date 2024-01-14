using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LFGameObjectType
{
    None,
    Object,
    NPC,
    Portal,
    Spawner,
    AutoDialog,
};

[Serializable]
public class InteractionTalkData
{
    public int TalkKey = 0;
    public List<string> TalkDataList;
}

public class LFGameObject : MonoBehaviour
{
    public int id;
    public string DisplayName;
    public LFGameObjectType lFGameObjectType;

    public int TalkIndex = 0;
    public List<string> DefaultInteractionTalkData;
    public List<InteractionTalkData> InteractionTalkData;
}