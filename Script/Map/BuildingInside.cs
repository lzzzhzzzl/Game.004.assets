using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class BuildingInside : MonoBehaviour
{
    public string buildingID;
    public BuildingInsideDetailData_SO buildingInsideDetails;
    /// <summary>
    /// 当前瓦片地图
    /// </summary>
    private Tilemap[] subjectTilemap;
    private Tilemap[] gridProperties;
    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            subjectTilemap = new Tilemap[gameObject.transform.GetChild(0).childCount];
            for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
                subjectTilemap[i] = gameObject.transform.GetChild(0).GetChild(i).GetComponent<Tilemap>();

            gridProperties = new Tilemap[gameObject.transform.GetChild(1).childCount];
            for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
                gridProperties[i] = gameObject.transform.GetChild(1).GetChild(i).GetComponent<Tilemap>();

            if (buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID) != null)
            {
                buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID).tilePropertiesList.Clear();
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            subjectTilemap = new Tilemap[gameObject.transform.GetChild(0).childCount];
            for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
                subjectTilemap[i] = gameObject.transform.GetChild(0).GetChild(i).GetComponent<Tilemap>();

            gridProperties = new Tilemap[gameObject.transform.GetChild(1).childCount];
            for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
                gridProperties[i] = gameObject.transform.GetChild(1).GetChild(i).GetComponent<Tilemap>();

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

        List<TilePropertyList> tilePropertiesList = new List<TilePropertyList>();
        for (int i = 0; i < gameObject.transform.GetChild(0).childCount; i++)
        {
            subjectTilemap[i].CompressBounds();
            if (!Application.IsPlaying(this))
            {
                if (buildingInsideDetails != null)
                {
                    TilePropertyList tileProperties = new TilePropertyList();

                    Vector3Int startPos = subjectTilemap[i].cellBounds.min;
                    Vector3Int endPos = subjectTilemap[i].cellBounds.max;

                    for (int x = startPos.x; x < endPos.x; x++)
                        for (int y = startPos.y; y < endPos.y; y++)
                        {
                            TileBase tile = subjectTilemap[i].GetTile(new Vector3Int(x, y, 0));

                            if (tile != null)
                            {
                                tileProperties.tileProperties.Add(new TileProperty(new SerializableVector3(x, y, 0), tile.name));
                            }
                        }
                    tilePropertiesList.Add(tileProperties);
                    Debug.Log("建筑ID(內部):" + buildingID + "   瓦片地图层:" + subjectTilemap[i].name + "   数量：" + tileProperties.tileProperties.Count + "   刷新完毕");
                }
            }
        }
        for (int i = 0; i < gameObject.transform.GetChild(1).childCount; i++)
        {
            gridProperties[i].CompressBounds();
            if (!Application.IsPlaying(this))
            {
                if (buildingInsideDetails != null)
                {
                    TilePropertyList tileProperties = new TilePropertyList();

                    Vector3Int startPos = gridProperties[i].cellBounds.min;
                    Vector3Int endPos = gridProperties[i].cellBounds.max;

                    for (int x = startPos.x; x < endPos.x; x++)
                        for (int y = startPos.y; y < endPos.y; y++)
                        {
                            TileBase tile = gridProperties[i].GetTile(new Vector3Int(x, y, 0));

                            if (tile != null)
                            {
                                tileProperties.tileProperties.Add(new TileProperty(new SerializableVector3(x, y, 0), tile.name));
                            }
                        }
                    tilePropertiesList.Add(tileProperties);
                    Debug.Log("建筑ID(內部):" + buildingID + "   瓦片地图层:" + gridProperties[i].name + "   数量：" + tileProperties.tileProperties.Count + "   刷新完毕");
                }
            }
        }

        buildingInsideDetails.buildingInsideList.Find(i => i.buildingID == buildingID).tilePropertiesList = tilePropertiesList;
        Debug.Log("建筑ID(內部):" + buildingID + "   刷新完毕");
    }
}