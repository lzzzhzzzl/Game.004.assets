using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public string cropID;
    private CropDetail cropDetail;
    private Animator anim;
    private Transform playerTransform => FindObjectOfType<Player>().transform;
    private int harvestActionCount;
    private void Start()
    {
        if (cropID != "")
        {
            Init(cropID);
        }
    }
    private void Init(string ID)
    {
        cropID = ID;
        anim = GetComponentInChildren<Animator>();
        cropDetail = InventoryManager.Instance.GetCropDetail(cropID);
    }
    public bool ProcessToolAction(ItemDetail tool)
    {
        int requireActionCount = cropDetail.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1)
            return false;
        if (cropDetail.hasParticalEffect)
            EventHandler.CallParticleGenerateEvent(transform.position, cropDetail.particalEffect, 0);
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            if (anim != null && cropDetail.hasAnimation)
            {
                if (playerTransform.position.x <= transform.position.x)
                {
                    anim.SetTrigger("BeCutRight");
                }
                else
                {
                    anim.SetTrigger("BeCutLeft");
                }
            }
        }
        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetail.hasAnimation)
            {
                if (playerTransform.position.x < transform.position.x)
                {
                    anim.SetTrigger("FallingRight");
                }
                else
                {
                    anim.SetTrigger("FallingLeft");
                }
                StartCoroutine(HarvestAfterAnimation());
            }
            else
            {
                SpawHarvestItem();
                if (cropDetail.transferCropID != "")
                {
                    CreatTransferCrop();
                }
                Destroy(gameObject);
            }
        }
        return true;
    }
    private IEnumerator HarvestAfterAnimation()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }

        SpawHarvestItem();
        if (cropDetail.transferCropID != "")
        {
            CreatTransferCrop();
        }
    }
    private void SpawHarvestItem()
    {
        for (int i = 0; i < cropDetail.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropDetail.productedMaxAmount[i] == cropDetail.productedMinAmount[i])
            {
                amountToProduce = cropDetail.productedMaxAmount[i];
            }
            else
            {
                amountToProduce = Random.Range(cropDetail.productedMinAmount[i], cropDetail.productedMaxAmount[i] + 1);
            }

            for (int j = 0; j < amountToProduce; j++)
            {
                var dirX = transform.position.x > playerTransform.position.x ? 1 : -1;
                var spawPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetail.spwamRadius.x * dirX),
                transform.position.y + Random.Range(-cropDetail.spwamRadius.y, cropDetail.spwamRadius.y), 0);
                EventHandler.CallGenerateItemEvent(cropDetail.producedItemID[i], spawPos);
            }
        }
    }
    private void CreatTransferCrop()
    {
        if (cropDetail.transferCropID != "")
            EventHandler.CallGenerateCropEvent(cropDetail.transferCropID, transform.position);
        Destroy(gameObject);
    }
}
