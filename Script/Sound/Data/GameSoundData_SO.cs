using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSoundData_SO", menuName = "Sound/GameSoundData_SO", order = 0)]
public class GameSoundData_SO : ScriptableObject
{
    public List<SoundDetail> soundDetailList;
    public SoundDetail GetGameSoundDetail(SoundName soundName)
    {
        return soundDetailList.Find(i => i.soundName == soundName);
    }
}