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
};


public class LFGameObject : MonoBehaviour
{
    public int id;
    public LFGameObjectType lFGameObjectType;
}