using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameSaveData
{
    public SerializableVector3 playerPosition;
    /// <summary>
    /// 场景的物品ID
    /// </summary>
    public Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
    /// <summary>
    /// 场景的可持有物品ID
    /// </summary>
    public Dictionary<string, List<ScenenCrop>> sceneCropDict = new Dictionary<string, List<ScenenCrop>>();
    /// <summary>
    /// 场景的可破坏物品ID
    /// </summary>
    public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();
    public Dictionary<string, int> sceneFurnitureIndexDict = new Dictionary<string, int>();
    /// <summary>
    /// 场景的可建造物品ID
    /// </summary>
    public Dictionary<string, List<SceneBuilding>> sceneBuildingDict = new Dictionary<string, List<SceneBuilding>>();

    /// <summary>
    /// 背包物品
    /// </summary>
    public List<InventoryItem> equipmentList = new List<InventoryItem>();
    public List<InventoryItem> inventoryDict = new List<InventoryItem>();
    public Dictionary<string, List<InventoryItem>> boxItemList = new Dictionary<string, List<InventoryItem>>();
    public Dictionary<string, int> buildingIDnumberDict = new Dictionary<string, int>();
    /// <summary>
    /// 时间相关参数
    /// </summary>
    public Dictionary<string, int> timeDict = new Dictionary<string, int>();
    /// <summary>
    /// 场景中NPC的坐标
    /// </summary>
    public Dictionary<string, List<SceneParameter>> sceneParameterDict = new Dictionary<string, List<SceneParameter>>();
    public Dictionary<string, SceneNameSaved> sceneNameDict = new Dictionary<string, SceneNameSaved>();
    public List<GridSaveDetail> gridsaveDetailDict = new List<GridSaveDetail>();
    public float[,] gridBuildMap;

    public float health;
    public SceneNameSaved currentScene;
    public Dictionary<string, bool> sceneHasGenerated = new Dictionary<string, bool>();
}
