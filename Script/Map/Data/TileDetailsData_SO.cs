using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "TileDetailsData_SO", menuName = "Map/Tile/TileDetailsData_SO", order = 0)]
public class TileDetailsData_SO : ScriptableObject
{
    public List<TileDetail> tileDetails;
}