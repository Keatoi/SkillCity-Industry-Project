using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    [SerializeField] private Dictionary<int, Item> items;

    //Checks for an instance of this script in game

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            items = new Dictionary<int, Item>();
        }
        else Destroy(gameObject);
    }

    //Adds specific items to inventory

    public void AddItem(ItemData itemData, int quantity)
    {
        if (items.ContainsKey(itemData.itemID))
        {
            items[itemData.itemID].Quantity += quantity;
        }
        else items[itemData.itemID] = new Item(itemData.itemID, itemData.name, quantity);
    }

    //Removes specific items from inventory

    public void RemoveItem(ItemData itemData, int quantity)
    {
        if (items.ContainsKey(itemData.itemID))
        {
            Item item = items[itemData.itemID];
            if(item.Quantity >= quantity)
            {
                item.Quantity -= quantity;

                if(item.Quantity <= 0)
                {
                    items.Remove(itemData.itemID);
                }
            }
        }
    }
}

public class Item
{
    public int ID { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; set; }

    public Item(int id, string name, int quantity)
    {
        ID = id;
        Name = name;
        Quantity = quantity;
    }
}
