using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "BulePrintData_SO", menuName = "Inventory/Furniture/BulePrintData_SO", order = 0)]
public class BulePrintData_SO : ScriptableObject
{
    public List<BluePrintDetail> bluePrintDataList;
    public BluePrintDetail GetBluePrintDetails(string furnitureID)
    {
        return bluePrintDataList.Find(b => b.furnitureID == furnitureID);
    }
}