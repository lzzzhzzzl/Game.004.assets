using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class InventoryUI : MonoBehaviour
{
    [Header("拖拽物品")]
    [SerializeField] public Image dargItem;

    [Header("玩家背包UI")]
    [SerializeField] private GameObject bagUI;
    private bool bagOpen;

    [Header("背包数组")]
    [SerializeField] private SlotUI[] playerSlots;
    [Header("装备数组")]
    [SerializeField] private SlotUI[] equipmentSlots;
    [Header("通用背包")]
    [SerializeField] private GameObject bagBase;
    [SerializeField] private GameObject slot_Box;

    private List<SlotUI> baseBagSlotList = new List<SlotUI>();
    private void OnEnable()
    {
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.UpdateInventoryUIEvent += OnUpdateInventoryUIEvent;
        EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
        EventHandler.UpdateInventoryUIEvent -= OnUpdateInventoryUIEvent;
        EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
    }

    private void OnBeforeSceneLoadEvent()
    {
        UpdateSlotHightLight(-1);
    }

    private void Start()
    {
        //给每个格子一个ID
        for (int i = 0; i < playerSlots.Length; i++)
        {
            playerSlots[i].slotIndex = i;
        }
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].slotIndex = i;
        }
        bagOpen = bagUI.activeInHierarchy;

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            EventHandler.CallChangePlayerEquiment(equipmentSlots[i].itemDetail);
        }
        EventHandler.CallChangeCharacterAnimatorEvent();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenBagUI();
        }
    }
    /// <summary>
    /// 打开背包
    /// </summary>
    public void OpenBagUI()
    {
        bagOpen = !bagOpen;
        bagUI.SetActive(bagOpen);
        InventoryManager.Instance.ChangePlayerEquiment();
    }
    /// <summary>
    /// 更新格子的高亮情况
    /// </summary>
    public void UpdateSlotHightLight(int index)
    {
        foreach (var slot in playerSlots)
        {
            if (slot.isSelected && slot.slotIndex == index)
            {
                slot.slotHightLight.gameObject.SetActive(true);
            }
            else
            {
                slot.isSelected = false;
                slot.slotHightLight.gameObject.SetActive(false);
            }
        }
    }
    private void OnUpdateInventoryUIEvent(InventoryLocation location, List<InventoryItem> list)
    {
        switch (location)
        {
            case InventoryLocation.Player:
                for (int i = 0; i < playerSlots.Length; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        var item = InventoryManager.Instance.GetItemDetail(list[i].itemID);
                        playerSlots[i].UpdateSlot(item, list[i].itemAmount, list[i].itemUseTimes);
                    }
                    else
                    {
                        playerSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Equipment:
                for (int i = 0; i < equipmentSlots.Length; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        var item = InventoryManager.Instance.GetItemDetail(list[i].itemID);
                        equipmentSlots[i].UpdateSlot(item, list[i].itemAmount, list[i].itemUseTimes);
                    }
                    else
                    {
                        equipmentSlots[i].UpdateEmptySlot();
                    }
                }
                break;
            case InventoryLocation.Box:
                for (int i = 0; i < baseBagSlotList.Count; i++)
                {
                    if (list[i].itemAmount > 0)
                    {
                        var item = InventoryManager.Instance.GetItemDetail(list[i].itemID);
                        baseBagSlotList[i].UpdateSlot(item, list[i].itemAmount, list[i].itemUseTimes);
                    }
                    else
                    {
                        baseBagSlotList[i].UpdateEmptySlot();
                    }
                }
                break;
        }

    }

    /// <summary>
    /// 加载场景前将背包的高亮关闭
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        UpdateSlotHightLight(-1);
    }
    private void OnBaseBagOpenEvent(SlotType slotType, InventorySlotData_SO bagData)
    {
        GameObject slotPrefab = slotType switch
        {
            SlotType.Box => slot_Box,
            _ => null
        };
        List<InventoryItem> itemList = bagData.itemList;
        for (int i = 0; i < bagData.itemList.Count; i++)
        {
            SlotUI slot = Instantiate(slotPrefab, bagBase.transform.GetChild(0)).GetComponent<SlotUI>();
            slot.slotIndex = i;
            baseBagSlotList.Add(slot);
        }

        bagBase.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bagBase.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(bagBase.transform.GetChild(0).GetComponent<RectTransform>());
        OnUpdateInventoryUIEvent(InventoryLocation.Box, bagData.itemList);
    }

    private void OnBaseBagCloseEvent(SlotType slotType, InventorySlotData_SO bagData)
    {
        bagBase.SetActive(false);
        UpdateSlotHightLight(-1);

        foreach (var slot in baseBagSlotList)
        {
            Destroy(slot.gameObject);
        }

        baseBagSlotList.Clear();
    }

}