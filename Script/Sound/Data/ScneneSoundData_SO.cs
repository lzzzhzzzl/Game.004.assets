using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScneneSoundData_SO", menuName = "Sound/ScneneSoundData_SO", order = 0)]
public class ScneneSoundData_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundItemList;
    public SceneSoundItem GetSceneSoundItem(string sceneName)
    {
        return sceneSoundItemList.Find(i => i.sceneName == sceneName);
    }
}