using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 常量表
/// </summary>
public class Settings
{
    /// <summary>
    /// 地图生成时，柏林噪音的偏移量的大小
    /// </summary>
    public const int seedSize = 10000;
    public const int itemMaxNumber = 64;
    public const float parameterHurtStateTime = 1f;
    /// <summary>
    /// 物品透明化的速度
    /// </summary>
    public const float itemFadeDuration = 0.35f;
    /// <summary>
    /// 物品透明化的程度
    /// </summary>
    public const float targetAlpha = 0.45f;

    public const float fadeDuration = 1.5f;
    public const float durationInHealth = 0.5f;
    public const float playerMaxHealth = 100f;
    public const float bonfireMaxHealth = 1000f;
    public const float secondThreshold = 0.01f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;

    public const float lightChangeDuration = 25f;
    public static TimeSpan mornimgTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

}
