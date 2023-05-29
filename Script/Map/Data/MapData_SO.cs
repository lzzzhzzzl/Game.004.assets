using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData_SO", order = 0)]
public class MapData_SO : ScriptableObject
{
    public List<MapData> mapDataList = new List<MapData>();
}