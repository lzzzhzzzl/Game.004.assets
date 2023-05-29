using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class BulePrintTypeSlot : MonoBehaviour, IPointerClickHandler
{
    public FurnitureType furnitureType;
    public BulePrintUI bulePrintUI => GetComponentInParent<BulePrintUI>();

    private void Start()
    {
        if (furnitureType == FurnitureType.Decoration)
            bulePrintUI.SelectFurnitureType(furnitureType);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        bulePrintUI.SelectFurnitureType(furnitureType);
    }
}
