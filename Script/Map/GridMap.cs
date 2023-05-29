using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public string buildingID;
    public BuildingExteriorDetailsData_SO buildingItemDetails;
    /// <summary>
    /// 当前瓦片地图
    /// </summary>
    private Tilemap currentTilemap;
    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (buildingItemDetails.buildingExteriorList.Find(i => i.buildingID == buildingID) != null)
            {
              //  buildingItemDetails.buildingExteriorList.Find(i => i.buildingID == buildingID).tilePropertyList.tileProperties.Clear();
            }
        }
    }
    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileProperties();
#if UNITY_EDITOR
            if (buildingItemDetails.buildingExteriorList != null)
            {
                EditorUtility.SetDirty(buildingItemDetails);
            }
#endif
        }
    }
    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (buildingItemDetails != null)
            {
                if (buildingItemDetails.buildingExteriorList.Find(i => i.buildingID == buildingID) == null)
                    buildingItemDetails.buildingExteriorList.Add(new BuildingExteriorDetail(buildingID));

                Vector3Int startPos = currentTilemap.cellBounds.min;
                Vector3Int endPos = currentTilemap.cellBounds.max;

                for (int x = startPos.x; x < endPos.x; x++)
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                           // buildingItemDetails.buildingExteriorList.Find(i => i.buildingID == buildingID).tilePropertyList.tileProperties.Add(x + "+" + y + "+" + tile.name);
                        }
                    }
                Debug.Log("物品ID:" + buildingID + "   刷新完毕");
            }
        }
    }
}

