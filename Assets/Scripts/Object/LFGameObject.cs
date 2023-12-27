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


public class LFGameObject : MonoBehaviour
{
    public int id;
    public string DisplayName;
    public LFGameObjectType lFGameObjectType;
}