using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDetails_SO", menuName = "Inventory/Item/ItemDetails_SO", order = 0)]
public class ItemDetails_SO : ScriptableObject
{
    public List<ItemDetail> itemDetailList;
}
