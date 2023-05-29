using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapDetailsData_SO", menuName = "Map/MapDetails/MapDetailsData_SO", order = 0)]
public class MapDetailsData_SO : ScriptableObject
{
    public string mapID;
    [Header("地图基本参数")]
    public MapDetail mapDetail;
    [Header("地图层级参数")]
    public List<MapComponentDetail> mapLevelList;
    [Header("地图元素参数")]
    public List<MapComponentDetail> mapElementList;
}