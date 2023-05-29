using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "BuildingInsideDetailData_SO", menuName = "Inventory/Building/BuildingInsideDetailData_SO", order = 0)]
public class BuildingInsideDetailData_SO : ScriptableObject
{
    public List<BuildingInsideDetail> buildingInsideList;
}