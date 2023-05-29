using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemID;
    public ItemDetail itemDetail;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private BoxCollider2D coll;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if (itemID != "")
        {
            Init(itemID);
        }
    }
    public void Init(string ID)
    {
        itemID = ID;
        itemDetail = InventoryManager.Instance.GetItemDetail(ID);
        if (itemDetail != null)
        {
            if (itemDetail.hasAnimation)
                anim.runtimeAnimatorController = itemDetail.itemInstantiateAnimator;
            else
                anim.enabled = false;
            spriteRenderer.sprite = itemDetail.itemOnWorldSprite != null ? itemDetail.itemOnWorldSprite : itemDetail.itemIcon;
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            coll.size = newSize;
            coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
        }
    }

}
