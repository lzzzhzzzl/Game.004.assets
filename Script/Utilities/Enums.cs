using System.Net.NetworkInformation;
using System.ComponentModel;
/// <summary>
/// 地图的生成模式
/// </summary>
public enum MapGmodeType
{
    /// <summary>
    /// 柏林噪音-普通模式
    /// </summary>
    PerlinNoiseStandard,
    /// <summary>
    /// 柏林噪音-强化山峰模式
    /// </summary>
    PerlinNoiseIntensify
}

/// <summary>
/// 形状类型
/// </summary>
public enum ShapeType
{
    /// <summary>
    /// 圆形
    /// </summary>
    Roundness,
    /// <summary>
    /// 方形
    /// </summary>s
    Square
}
public enum ItemType
{
    Consumable, Material, AxeTool, PickAxe, Sword, Throw, Clothes
}
public enum SlotType
{
    Bag, Box, Equipment_Clothes
}
public enum BuildingType
{
    Building, Wall, Spawn
}
public enum BlockType
{
    Floor, Wall, Spawn, Building
}
public enum InventoryLocation
{
    Player, Box, Equipment
}
/// <summary>
/// 地图分类，用于判定粒子特效以及动画
/// </summary>
public enum MapPartType
{
    Ground, // 土地
    Beach,  // 沙滩
    Ocean,  // 海洋
    Lake,   // 湖泊
    Forest, // 森林
    Road    // 道路
}
/// <summary>
/// 动画分类，与地图分类或者与物品的交互确定该执行的动画
/// </summary>
public enum PlayerPartType
{
    None,    // 无
    Carry,   // 默认持握
    Hoe,     // 斧子
    Break,   // 镐子
    Sword,   // 刀剑
    Hurt,
    Die
}
/// <summary>
/// 角色身上可执行动画部位的分类
/// </summary>
public enum PlayerPartName
{
    Hair, Body, Clothes, Tool
}
public enum ParticaleEffectType
{
    None, HealthDamage, HealthRecover, Stone, Leaves, Grass, Gonden
}
public enum SoundName
{
    None, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Sword, Pickup,
    AmbientCountryside1, AmbientCountryside2, AmbientCountryside3, AmbientCountryside4,
    MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6,
    AmbientIndoor1, AmbientIndoor2,
    AmbientInCave1, AmbientInCave2
}
public enum BulletType
{
    undead_1, archer_1, undead_2, undead_3, undead_4, undead_5, undead_6
}
public enum StateType
{
    Appear, Idle, Move, Die, Attack, Skill, Hurt
}
public enum StateDataType
{
    EnemyAppear, EnemyIdle, EnemyMove, EnemyDie, EnemyHurt, EnemyAttack, EnemySkillRemote, EnemySkillSummon,
    TowerAppear, TowerIdle, TowerMove, TowerDie, Tower1Attack, Tower2Attack, Tower3Attack, TowerSkill,
    AnimalAppear, AnimalIdle, AnimalMove, AnimalDie, AnimalHurt, AnimalAttack, AnimalSkill,
    None
}
public enum LightShift
{
    Morning, Night
}
public enum MapComponent
{
    Water, Ground, Forset, Lake, Town
}
public enum MapLevel
{
    Water, Ground
}
public enum MapElement
{
    Forset, Lake, Town
}
public enum ParameterType
{
    Enemy, Tower, Animal
}
public enum FurnitureType
{
    Protection, Decoration, Function, Unbuildable
}
public enum CursorType
{
    Normal, Build, Remove, Islandmap, UseItem
}