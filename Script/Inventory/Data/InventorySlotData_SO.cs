using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "InventorySlotData_SO", menuName = "Inventory/Data/InventorySlot", order = 0)]
public class InventorySlotData_SO : ScriptableObject
{
    public List<InventoryItem> itemList;
    public InventoryItem GetInventoryItem(string ID)
    {
        return itemList.Find(i => i.itemID == ID);
    }
}