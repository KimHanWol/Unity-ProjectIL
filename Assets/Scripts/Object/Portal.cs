using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : LFGameObject
{
    public Portal()
    {
        lFGameObjectType = LFGameObjectType.Portal;
    }

    public LFGameObject SpawnObject;
}
