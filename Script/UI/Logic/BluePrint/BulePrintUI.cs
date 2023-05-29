using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BulePrintUI : Singleton<BulePrintUI>
{
    [Header("物品信息显示UI")]
    public Image itemImage;
    public Transform itemDetailList;
    public GameObject itemPrefab;
    public TMP_Text furnitureTypeText;
    [Header("蓝图列表")]
    public Transform itemSlotList;
    public GameObject itemSlot;
    [Header("建造按钮")]
    public Button buildButton;
    [Header("蓝图在物品栏的按钮")]
    public Button bluePrintButton;
    public Button removeFurnitureButton;
    [Header("蓝图的总UI")]
    public GameObject bulePrintUI;


    private bool isOpenBluePrint;
    private FurnitureType currentFurnitureType;
    private bool canbuild;
    private BluePrintDetail currentBluePrintDeatail;
    private List<BluePrintDetail> bluePrintDetailList;
    private List<BulePrintSlotUI> bluePrintSlotList = new List<BulePrintSlotUI>();
    private void OnEnable()
    {
        EventHandler.IsInCampSceneEvent += OnIsInCampSceneEvent;
        EventHandler.BluePrintSelectedEvent += OnBluePrintSelectedEvent;
    }
    private void OnDisable()
    {
        EventHandler.IsInCampSceneEvent -= OnIsInCampSceneEvent;
        EventHandler.BluePrintSelectedEvent -= OnBluePrintSelectedEvent;
    }

    private void Start()
    {
        bluePrintDetailList = InventoryManager.Instance.GetBulePrintDataList();
        BuildBluePrintItemList();
        OnBluePrintSelectedEvent(null);
    }

    private void OnIsInCampSceneEvent(bool isInCamp)
    {
        bluePrintButton.interactable = isInCamp;
    }
    private void OnBluePrintSelectedEvent(BluePrintDetail bluePrintDetail)
    {
        for (int i = 0; i < itemDetailList.childCount; i++)
        {
            GameObject.Destroy(itemDetailList.transform.GetChild(i).gameObject);
        }
        if (bluePrintDetail == null)
        {
            itemImage.enabled = false;
            buildButton.interactable = false;
        }
        else
        {
            canbuild = true;
            currentBluePrintDeatail = bluePrintDetail;
            itemImage.sprite = bluePrintDetail.furnitureSpriteInUI;
            itemImage.enabled = true;
            buildButton.interactable = true;
            for (int i = 0; i < bluePrintDetail.resourceItem.Length; i++)
            {
                GameObject bluePrintItem = Instantiate(itemPrefab, itemDetailList);

                if (!InventoryManager.Instance.CheckBagItemAmount(bluePrintDetail.resourceItem[i]))
                {
                    bluePrintItem.GetComponent<BluePrintItemDetail>().SetBluePtintDeatil(bluePrintDetail.resourceItem[i], false);
                    canbuild = false;
                }
                else
                    bluePrintItem.GetComponent<BluePrintItemDetail>().SetBluePtintDeatil(bluePrintDetail.resourceItem[i], true);
            }
        }
    }
    public void OnOpenBulePrint()
    {
        isOpenBluePrint = !isOpenBluePrint;
        bulePrintUI.SetActive(isOpenBluePrint);
    }
    private void BuildBluePrintItemList()
    {
        int index = 0;
        foreach (BluePrintDetail bluePrintDetail in bluePrintDetailList)
        {
            if (bluePrintDetail.furnitureType == currentFurnitureType)
            {
                GameObject bluePrintItem = Instantiate(itemSlot, itemSlotList);
                bluePrintItem.GetComponent<BulePrintSlotUI>().Init(bluePrintDetail, index);
                bluePrintSlotList.Add(bluePrintItem.GetComponent<BulePrintSlotUI>());
                index++;
            }
        }
    }
    public void UpdateSlotHightLight(int slotIndex)
    {
        foreach (BulePrintSlotUI bulePrintSlot in bluePrintSlotList)
        {
            if (bulePrintSlot.slotIndex == slotIndex)
            {
                bulePrintSlot.slotHightLight.enabled = true;
            }
            else
                bulePrintSlot.slotHightLight.enabled = false;
        }
    }
    public void SelectFurnitureType(FurnitureType furnitureType)
    {
        currentFurnitureType = furnitureType;
        furnitureTypeText.text = furnitureType switch
        {
            FurnitureType.Protection => "防御设施",
            FurnitureType.Decoration => "装饰",
            FurnitureType.Function => "工作",
            FurnitureType.Unbuildable => "无",
            _ => "无"
        };
        for (int i = 0; i < itemSlotList.childCount; i++)
        {
            GameObject.Destroy(itemSlotList.transform.GetChild(i).gameObject);
        }
        bluePrintSlotList.Clear();
        BuildBluePrintItemList();
    }
    public void BuildButtonClick()
    {
        if (currentBluePrintDeatail != null && canbuild)
        {
            OnOpenBulePrint();
            EventHandler.CallOpenBulePrintEvent(true);
            EventHandler.CallBuildBluePrintEvent(currentBluePrintDeatail);
            OnBluePrintSelectedEvent(null);
        }
    }
    public void RemoveButtonClick()
    {
        OnOpenBulePrint();
        EventHandler.CallOpenBulePrintEvent(true);
        EventHandler.CallRemoveButtonClickEvent();
    }
}
