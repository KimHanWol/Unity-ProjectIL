using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/Item", order = int.MaxValue)]
public class Item : ScriptableObject
{
    [SerializeField]
    public string itemID;

    [SerializeField]
    public string ItemName;

    [SerializeField]
    public Sprite ItemImage;
}
