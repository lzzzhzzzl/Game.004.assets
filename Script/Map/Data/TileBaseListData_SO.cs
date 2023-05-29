using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileBaseListData_SO", menuName = "Map/Tile/TileBaseListData_SO", order = 0)]
public class TileBaseListData_SO : ScriptableObject
{
    public List<TileBase> tileBaseDataList = new List<TileBase>();
}