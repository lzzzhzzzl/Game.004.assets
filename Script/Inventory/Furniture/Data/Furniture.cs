using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour
{
    public string furnitureID;
    private BluePrintDetail bluePrintDetail;
    private void Start()
    {
        if (furnitureID != "")
        {
            Init(furnitureID);
        }
    }
    private void Init(string ID)
    {
        furnitureID = ID;
        bluePrintDetail = InventoryManager.Instance.GetBluePrintDetail(furnitureID);
    }
}