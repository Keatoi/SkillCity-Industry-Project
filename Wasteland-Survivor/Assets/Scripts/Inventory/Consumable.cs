using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public ItemData itemData;
    public int amountUsed = 1;
    
    //This script to be used on objects in hand 
    public void UseItem()
    {
        InventoryManager.Instance.RemoveItem(itemData, amountUsed);
    }
}
