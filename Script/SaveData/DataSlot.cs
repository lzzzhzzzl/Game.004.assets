using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSlot
{
    public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

    public string DataTime
    {
        get
        {
            var key = TimeManager.Instance.GUID;

            if (dataDict.ContainsKey(key))
            {
                var timeData = dataDict[key];
                return timeData.timeDict["gameDay"] + "日/" + timeData.timeDict["gameHour"] + "时/" + timeData.timeDict["gameMinute"] + "分";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public string DataScene
    {
        get
        {
            var key = TransitionManager.Instance.GUID;

            if (dataDict.ContainsKey(key))
            {
                var transitionData = dataDict[key];
                return transitionData.currentScene.sceneName switch
                {
                    "Map-10001" => "资源岛屿",
                    "Map-10002" => "资源岛屿",
                    "Map-10003" => "资源岛屿",
                    "Map-10004" => "资源岛屿",
                    "Map-10005" => "神庙",
                    "Map-10006" => "营地岛屿",
                    "Building-10001" => "神庙祭坛内部",
                    _ => string.Empty
                };
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
