using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDialogObject : LFGameObject
{
    public AutoDialogObject()
    {
        lFGameObjectType = LFGameObjectType.AutoDialog;
    }

    public int DialogKey;
    public bool IsNPC;

    private bool isFirstInteraction = true;

    public bool IsFirstInteraction
    {
        get { return isFirstInteraction; }
        set { isFirstInteraction = value; }
    }
}
