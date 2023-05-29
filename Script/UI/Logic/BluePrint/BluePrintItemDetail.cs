using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BluePrintItemDetail : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemAmount;
    public void SetBluePtintDeatil(InventoryItem inventoryItem, bool active)
    {

        ItemDetail item = InventoryManager.Instance.GetItemDetail(inventoryItem.itemID);
        if (item != null)
        {
            itemImage.sprite = item.itemIcon;
            itemName.text = item.itemName;
            itemAmount.text = "x" + inventoryItem.itemAmount.ToString();
        }
        if (!active)
        {
            itemName.color = Color.red;
            itemAmount.color = Color.red;
        }
        else
        {
            itemName.color = Color.black;
            itemAmount.color = Color.black;
        }
    }

}
