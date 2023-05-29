using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Strategy.Map;

public class Building : MonoBehaviour
{
    public string buildingID;
    private BuildingExteriorDetail buildingDetail;
    private void Start()
    {
        if (buildingID != "")
        {
            Init(buildingID);
        }
    }
    private void Init(string ID)
    {
        buildingID = ID;
        buildingDetail = InventoryManager.Instance.GetBuildingExteriorDetail(buildingID);
    }
}
