using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    public List<Item> items;

    public List<Image> ItemBags;

    public void UpdateItemBags()
    {
        for(int i = 0; i < ItemBags.Count; i++) 
        {
            if(items.Count > i)
            {
                ItemBags[i].sprite = items[i].ItemImage;
            }
        }
    }
}
