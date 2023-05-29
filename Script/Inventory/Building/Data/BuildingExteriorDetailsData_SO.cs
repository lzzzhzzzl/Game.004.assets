using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BuildingExteriorDetailsData_SO", menuName = "Inventory/Building/BuildingItemDetailsData_SO", order = 0)]
public class BuildingExteriorDetailsData_SO : ScriptableObject
{
    public List<BuildingExteriorDetail> buildingExteriorList;
}
