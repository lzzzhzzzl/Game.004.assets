using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
 private void OnTriggerEnter2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                if (item.itemDetail.canPickedup == true)
                {
                    InventoryManager.Instance.AddItem(item, true);

                   // EventHandler.CallPlaySoundEffect(SoundName.Pickup);
                }
            }
        }
}
