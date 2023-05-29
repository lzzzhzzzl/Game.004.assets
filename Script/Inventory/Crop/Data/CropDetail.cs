using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CropDetail
{
    public string cropID;
    [Header("收割工具以及其对应的采集次数")]
    public string[] harvestToolItemID;
    public int[] requireActionCount;
    [Space]
    [Header("采集后生成物品的ID")]
    public string[] producedItemID;
    public int[] productedMaxAmount;
    public int[] productedMinAmount;
    public Vector2 spwamRadius;
    [Space]
    [Header("采集前后的物品")]
    public GameObject cropPrefab;
    public string transferCropID;
    [Space]
    [Header("其他设置")]
    public bool hasAnimation;
    /// <summary>
    /// 有没有粒子特效
    /// </summary>
    public bool hasParticalEffect;

    /// <summary>
    /// 特效的类型
    /// </summary>
    public ParticaleEffectType particalEffect;

    /// <summary>
    /// 生成粒子特效的坐标
    /// </summary>
    public Vector3 effectPos;
    public SoundName soundEffect;
    public int GetTotalRequireCount(string itemID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
            if (harvestToolItemID[i] == itemID)
                return requireActionCount[i];
        return -1;
    }
}
