using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnDir
{ 
    Left,
    Right,
    Up,
    Down,
};


public class Spawner : LFGameObject
{
    public SpawnDir SpawnDirection;
}
