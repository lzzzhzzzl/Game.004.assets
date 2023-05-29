using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulePrintSlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image slotImage;
    public Image slotHightLight;
    public Button bagButton;
    private bool isSelected;
    public int slotIndex;
    private BluePrintDetail bluePrintDetail;
    public BulePrintUI bulePrintUI => GetComponentInParent<BulePrintUI>();
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    public void Init(BluePrintDetail Detail, int index)
    {
        slotIndex = index;
        bluePrintDetail = Detail;
        slotImage.sprite = bluePrintDetail.furnitureSpriteInUI;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (bluePrintDetail == null)
            return;
        isSelected = !isSelected;
        bulePrintUI.UpdateSlotHightLight(slotIndex);

        EventHandler.CallBluePrintSelectedEvent(bluePrintDetail);
    }
}
