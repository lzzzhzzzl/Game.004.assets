using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Strategy.Map;
using Strategy.Astar;
using System.Collections.Generic;

/// <summary>
/// 地图的构建信息
/// </summary>
[System.Serializable]
public class MapDetail
{
    /// <summary>
    /// 地图的名称
    /// </summary>
    public string name;
    /// <summary>
    /// 地图的描述
    /// </summary>
    public string describe;
    /// <summary>
    /// 地图的尺寸
    /// </summary>
    public Vector2Int _mapSize;
    /// <summary>
    /// 单个区块的大小
    /// </summary>
    public int blockHeight;
    [Header("地图调整参数")]
    /// <summary>
    /// 地图的缩放值
    /// </summary>
    public float scale;
    /// <summary>
    /// 生成地图时的参数，表示生成地图时山峰的高度
    /// </summary>
    public float lacunarity;
    /// <summary>
    /// 生成地图时的参数，表示生成地图时山峰的宽度
    /// </summary>
    public float persistance;
    /// <summary>
    /// 生成地图时，使用amplitude和persistance修改地图的次数
    /// </summary>
    public int octaves;
    /// <summary>    
    /// 地图的权重
    /// </summary>
    [Range(0, 1)]
    public float weight;
    /// <summary>
    /// 地图的生成模式
    /// </summary>
    public MapGmodeType mapGenerationmode;
    [Header("地形与建筑")]
    /// <summary>
    /// 地形的信息列表
    /// </summary>
    public List<Landform> landformList;

}
/// <summary>
/// 地形信息,用于在基础的地图层上绘制岛屿
/// </summary>
[System.Serializable]
public class Landform
{
    /// <summary>
    /// 生成地形点的中心坐标
    /// </summary>
    public Vector2Int pos;
    /// <summary>
    /// 生成地形的形状
    /// </summary>
    public ShapeType shape;
    /// <summary>
    /// 生成地形的长度
    /// </summary>
    public int length;
    /// <summary>
    /// 生成地形的权重
    /// </summary>
    [Range(0, 1)]
    public float weight;
    public bool incersionValue;
    [Header("在地图上生成建筑时的范围")]
    public Vector2 buildingRadius;
    public bool inversionX;
}
/// <summary>
/// 大地图的层级瓦片
/// </summary>
[System.Serializable]
public class TileDetail
{
    public string blockID;
    public TileBase tile;
}
/// <summary>
/// 地图组件信息
/// </summary>
[System.Serializable]
public class MapComponentDetail
{
    public string blockID;
    public string name;
    public MapComponent mapComponent;
    public float scale;
    public int minisize;
    public Vector2 range;
}
/// <summary>
/// 地图瓦片
/// </summary>
[System.Serializable]
public class MapBlock
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector2Int pos;
    /// <summary>
    /// 该位置的地图组件
    /// </summary>
    public MapComponentDetail mapComponentDetail;
    public MapBlock(Vector2Int pos)
    {
        this.pos = pos;
        this.mapComponentDetail = new MapComponentDetail();
    }
    public void AddMapComponent(MapComponentDetail mapComponentDetail)
    {
        this.mapComponentDetail = mapComponentDetail;
    }
}
/// <summary>
///  区块信息
/// </summary>
[System.Serializable]
public class BlockDetail
{
    public string blockId;
    public BlockType blockType;
    [Header("这个区块生成物体在总体的比例")]
    [Range(0, 0.5f)]
    public float GenerationRatio;

    [Header("这个区块ID中可以生成的物体的ID")]
    public string[] itemsID;
}
/// <summary>
/// 物品信息
/// </summary>
[System.Serializable]
public class ItemDetail
{
    public string itemID;
    /// <summary>
    /// 物品的名字
    /// </summary>
    public string itemName;
    /// <summary>
    /// 物品的类型
    /// </summary>
    public ItemType itemType;
    /// <summary>
    /// 物品的图标
    /// </summary>
    public Sprite itemIcon;
    /// <summary>
    /// 物品显示在世界上的图标
    /// </summary>
    public Sprite itemOnWorldSprite;
    /// <summary>
    /// 物品可使用的范围
    /// </summary>
    public int itemUseRadius;

    public bool canPickedup;
    public bool hasAnimation;
    public AnimatorOverrideController itemInstantiateAnimator;
    public int damage;
    public int useTimes;
}
/// <summary>
/// 格子信息-用于背包、箱子、商店中的格子中
/// </summary>
[System.Serializable]
public struct InventoryItem
{
    /// <summary>
    /// 物品的ID
    /// </summary>
    public string itemID;
    /// <summary>
    /// 物品的数量
    /// </summary>
    public int itemAmount;
    public int itemUseTimes;
}
/// <summary>
/// 场景的中物品的信息
/// </summary>
[System.Serializable]
public class SceneItem
{
    public string ItemID;
    public SerializableVector3 position;
}
/// <summary>
/// 场景的中建筑的信息
/// </summary>
[System.Serializable]
public class SceneBuilding
{
    public string buildingID;
    public SerializableVector3 position;
    public int teleportSeed;
    public string teleportSceneName;
    public string teleportMapID;
    public SerializableVector3 targetPosition;
}
/// <summary>
/// 场景的中作物（可采集物品）的信息
/// </summary>
[System.Serializable]
public class ScenenCrop
{
    public string cropID;
    public SerializableVector3 position;
}
[System.Serializable]
public class SceneFurniture
{
    public string furnitureID;
    public SerializableVector3 position;
    public int index;
}
/// <summary>
/// 建筑外部物品信息
/// </summary>
[System.Serializable]
public class BuildingExteriorDetail
{
    public string buildingID;
    public Sprite sprite;
    public GameObject buildingPrefab;
    public BuildingType buildingType;
    public BuildingExteriorDetail(string buildingID)
    {
        this.buildingID = buildingID;
    }
}
/// <summary>
/// 建筑内部信息列表
/// </summary>
[System.Serializable]
public class BuildingInsideDetail
{
    public string buildingID;
    public GameObject buildingPrefab;
    public List<TilePropertyList> tilePropertiesList;
    public List<SerializableVector3> inventoryPositionList;
    public string[] inventoryList;
    public BuildingInsideDetail(string buildingID)
    {
        this.buildingID = buildingID;
        tilePropertiesList = new List<TilePropertyList>();
    }
}
/// <summary>
/// 瓦片地图信息列表，方便保存，Json无法存储TileBase
/// </summary>
[System.Serializable]
public class TilePropertyList
{
    public List<TileProperty> tileProperties;
    public TilePropertyList()
    {
        tileProperties = new List<TileProperty>();
    }
}
[System.Serializable]
public class TileProperty
{
    public SerializableVector3 position;
    public string tileName;
    public TileProperty(SerializableVector3 position, string tileName)
    {
        this.position = position;
        this.tileName = tileName;
    }
}
/// < summary >
/// 可序列化的三位变量，方便保存，Json无法存储Vector3
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }
    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public SerializableVector3() { }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public Vector3Int ToVector3Int()
    {
        return new Vector3Int((int)x, (int)y, (int)z);
    }
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}
/// <summary>
/// 瓦片地图的保存信息，包含数个瓦片地图信息列表（TilePropertyList）以及场景的名称，用于保存场景的瓦片
/// </summary>
[System.Serializable]
public class GridSaveDetail
{
    public string sceneName;
    public Dictionary<string, TilePropertyList> tilemaps;
    public GridSaveDetail()
    {
        sceneName = "null";
        tilemaps = new Dictionary<string, TilePropertyList>();
    }
}
/// <summary>
/// 地图的区块信息
/// </summary>
[System.Serializable]
public class Block
{
    public BlockDetail blockDetail;
    public float[,] blockRange;
    public int quantity
    {
        get
        {
            int num = 0;
            for (int x = 0; x < blockRange.GetLength(0); x++)
                for (int y = 0; y < blockRange.GetLength(1); y++)
                    if (blockRange[x, y] == 1)
                        num++;
            return (int)(num * (blockDetail.GenerationRatio / 4));
        }
    }
    public Block(BlockDetail blockDetail, float[,] blockRange)
    {
        this.blockDetail = blockDetail;
        this.blockRange = blockRange;
    }
}
/// <summary>
/// 动画信息
/// </summary>
[System.Serializable]
public class AnimatorType
{
    /// <summary>
    /// 动画的类型
    /// </summary>
    public PlayerPartType partType;

    /// <summary>
    /// 要执行动画的位置
    /// </summary>
    public PlayerPartName partName;

    /// <summary>
    /// 动画
    /// </summary>
    public AnimatorOverrideController overrideController;
}
[System.Serializable]
public class AnimatorTypeGroup
{
    public string ID;
    public List<AnimatorType> animatorTypes;
}
[System.Serializable]
public class CharacterDetail
{
    public string clothesID;
    public string bodyID;
    public string hairID;
    public string toolID;
}
[System.Serializable]
public class BluePrintDetail
{
    public string furnitureID;
    public Vector3Int size;
    public FurnitureType furnitureType;
    public InventoryItem[] resourceItem;
    public bool canBuild;
    public Sprite furnitureSpriteInUI;
    public GameObject furnitureBuilding;
}
[System.Serializable]
public class Parameter
{
    public string ID;
    public ParameterType parameterType;
    public StateDataList stateDataList;
    public float checkRange;
    public float checkTime;
    public float health;
    public float moveSpeed;
    public Vector3 MeleeAttackPosition;
    public float MeleeAttackRadius;
    public float MeleeAttackDamage;
    public float RemoteAttackRadius;
    public float RemoteAttackTime;
    public BulletType bulletType;
    public string[] itemCanGenerate;
    public int[] itemMaxCount;
    public int[] itemMinCount;
    public bool hasAppearAnimal;
    public string generateParameterID;
    public AnimatorOverrideController animatorOverride;
}
[System.Serializable]
public class StateDataList
{
    public StateDataType appear;
    public StateDataType idle;
    public StateDataType move;
    public StateDataType hurt;
    public StateDataType die;
    public StateDataType attack;
    public StateDataType skill;
}
[System.Serializable]
public class LightDetail
{
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}
[System.Serializable]
public class MapData
{
    public string mapID;
    [Multiline(5)]
    public string describe;
    public string fromScene;
    public Sprite sprite;
    public Vector3 startPos;
    public TileBase mapTile;
    public bool isRandomMap;
    public bool isCamp;
}
[System.Serializable]
public class SceneDetail
{
    public int seed;
    public string mapID;
    public string targetScene;
    public Sprite sprite;
    public Vector3 startPos;
    public bool isRandomMap;
    public SceneDetail(int seed, string mapID, string targetScene, Sprite sprite, Vector3 startPos, bool isRandomMap)
    {
        this.seed = seed;
        this.mapID = mapID;
        this.targetScene = targetScene;
        this.sprite = sprite;
        this.startPos = startPos;
        this.isRandomMap = isRandomMap;
    }
    public SceneDetail() { }
}
[System.Serializable]
public class SceneParameter
{
    public string parmeterID;
    public SerializableVector3 position;
    public float health;
}
[System.Serializable]
public class InstanceDetail
{
    public string parameterID;
    public int gameHour;
    public int gameDay;
    public float speed;
    public float amount;
}
[System.Serializable]
public class SceneNameSaved
{
    public int seed;
    public string sceneName;
    public SceneNameSaved(int seed, string sceneName)
    {
        this.seed = seed;
        this.sceneName = sceneName;
    }
}
[System.Serializable]
public class SoundDetail
{
    public SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)]
    public float soundPitchMin;
    [Range(0.1f, 1.5f)]
    public float soundPitchMax;
    [Range(0.1f, 1f)]
    public float soundVolime;
}
[System.Serializable]
public class SceneSoundItem
{
    public string sceneName;
    public SoundName ambient;
    public SoundName music;
}
