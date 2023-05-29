using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFurniturePoint : MonoBehaviour
{
    public string furnitureID;
    private void Start()
    {
        GenerateFurniture(furnitureID);
    }
    public void GenerateFurniture(string furnitureID)
    {
        BluePrintDetail bluePrintDetail = InventoryManager.Instance.GetBluePrintDetail(furnitureID);
        EventHandler.CallGenerateFurnitureEvent(bluePrintDetail, transform.position);
        Destroy(gameObject);
    }
}
