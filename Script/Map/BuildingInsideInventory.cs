using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class BuildingInsideInventory : MonoBehaviour
{
    public string buildingID;
    public BuildingInsideDetailData_SO buildingInsideDetails;
    /// <summary>
    /// 当前瓦片地图
    /// </summary>
    private Tilemap inventoryPosition;
    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            inventoryPosition = gameObject.transform.GetChild(0).GetComponent<Tilemap>();
            if (buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID) != null)
            {
                buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID).inventoryPositionList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            inventoryPosition = gameObject.transform.GetChild(0).GetComponent<Tilemap>();
            UpdateTileProperties();
#if UNITY_EDITOR
            if (buildingInsideDetails.buildingInsideList != null)
            {
                EditorUtility.SetDirty(buildingInsideDetails);
            }
#endif
        }
    }

    private void UpdateTileProperties()
    {
        if (buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID) == null)
            buildingInsideDetails.buildingInsideList.Add(new BuildingInsideDetail(buildingID));

        inventoryPosition.CompressBounds();

        if (!Application.IsPlaying(this))
        {
            if (buildingInsideDetails != null)
            {
                List<SerializableVector3> inventoryPositionList = new List<SerializableVector3>();
                Vector3Int startPos = inventoryPosition.cellBounds.min;
                Vector3Int endPos = inventoryPosition.cellBounds.max;

                for (int x = startPos.x; x < endPos.x; x++)
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = inventoryPosition.GetTile(new Vector3Int(x, y, 0));

                        if (tile != null)
                        {
                            SerializableVector3 pos = new SerializableVector3(new Vector3Int(x, y, 0));
                            inventoryPositionList.Add(pos);
                        }
                    }

                buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID).inventoryPositionList = inventoryPositionList;
            }
        }
        Debug.Log("建筑ID(內部):" + buildingID + "   可放置物品区域刷新完毕");
    }
}
