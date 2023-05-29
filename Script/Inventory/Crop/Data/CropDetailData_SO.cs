using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "CropDetailData_SO", menuName = "Inventory/Crop/CropDetailData_SO", order = 0)]
public class CropDetailData_SO : ScriptableObject
{
    public List<CropDetail> cropDetailList;
}