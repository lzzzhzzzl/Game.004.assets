using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("组件获取")]
    public Slider slider;
    [Tooltip("背包格显示的物品图片")]
    [SerializeField] private Image slotImage;

    [Tooltip("背包格显示的物品数量")]
    [SerializeField] private TextMeshProUGUI amountText;

    [Tooltip("物品被选中时的高亮图像")]
    public Image slotHightLight;
    [Tooltip("背包按钮")]
    public Button bagButton;

    [Header("格子的信息")]
    public int slotIndex;
    public int itemAmount;
    public SlotType slotType;
    public ItemDetail itemDetail;
    public bool isSelected;

    public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetail == null)
            return;
        isSelected = !isSelected;
        inventoryUI.UpdateSlotHightLight(slotIndex);

        if (slotType == SlotType.Bag)
        {
            // 举起物品动画切换
            EventHandler.CallItemSelectedEvent(itemDetail, slotIndex, isSelected);
        }
    }

    /// <summary>
    /// 更新该格子的信息
    /// </summary>
    public void UpdateSlot(ItemDetail item, int amount, int useTimes)
    {
        //切换数据
        itemDetail = item;
        slotImage.sprite = item.itemIcon;
        itemAmount = amount;
        if (item.itemType == ItemType.AxeTool || item.itemType == ItemType.PickAxe || item.itemType == ItemType.Sword || item.itemType == ItemType.Clothes)
        {
            slider.gameObject.SetActive(true);
            amountText.enabled = false;
            slider.value = (float)useTimes / (float)item.useTimes;
        }
        else
        {
            amountText.enabled = true;
            amountText.text = itemAmount.ToString();
            slider.gameObject.SetActive(false);
        }

        //切换显示
        slotImage.enabled = true;
        bagButton.interactable = true;
    }
    /// <summary>
    /// 将格子更新为空 
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;
            inventoryUI.UpdateSlotHightLight(-1);
            EventHandler.CallItemSelectedEvent(itemDetail, slotIndex, isSelected);
        }

        // 更新信息为空
        itemDetail = null;
        slotImage.enabled = false;
        amountText.text = string.Empty;
        bagButton.interactable = false;
        slider.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetail != null) // 如果不为空，则设置拖拽物体，这里不可能去拖拽格子，要不就乱套了，所以有一个额外的图片，专门用来拖拽
        {
            // Debug.Log("StartDarg");
            slotImage.enabled = false;
            inventoryUI.dargItem.enabled = true;
            inventoryUI.dargItem.sprite = slotImage.sprite;
            inventoryUI.dargItem.SetNativeSize();

            isSelected = true;
            inventoryUI.UpdateSlotHightLight(slotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.dargItem.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotImage.enabled = true;
        inventoryUI.dargItem.enabled = false;

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                return;

            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            int targetIndex = targetSlot.slotIndex;

            if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
            {
                InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
            }
            else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Equipment_Clothes && itemDetail.itemType == ItemType.Clothes)
            {
                //WORKFLOW: 额外装备需要在这里添加，否则添加不到物品栏上
                InventoryManager.Instance.SwapItemWithEquipment(slotIndex, targetIndex, itemDetail);
            }
            else if (slotType == SlotType.Equipment_Clothes && targetSlot.slotType == SlotType.Bag)
            {
                //WORKFLOW: 额外装备需要在这里添加，否则添加不到物品栏上
                if (targetSlot.itemDetail == null || targetSlot.itemDetail.itemType == ItemType.Clothes)
                    InventoryManager.Instance.SwapEquipmentWithItem(slotIndex, targetIndex, targetSlot.itemDetail);
            }
            else if (slotType == SlotType.Box && targetSlot.slotType == SlotType.Bag)
            {
                InventoryManager.Instance.SwapItem(slotIndex, InventoryLocation.Box, targetIndex, InventoryLocation.Player);
            }
            else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Box)
            {
                InventoryManager.Instance.SwapItem(slotIndex, InventoryLocation.Player, targetIndex, InventoryLocation.Box);
            }
            else if (slotType == SlotType.Box && targetSlot.slotType == SlotType.Box)
            {
                InventoryManager.Instance.SwapItem(slotIndex, InventoryLocation.Box, targetIndex, InventoryLocation.Box);
            }
            inventoryUI.UpdateSlotHightLight(-1);
        }
    }
}
