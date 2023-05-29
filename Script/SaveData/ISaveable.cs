using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    public string GUID { get; }
    public void RegisterSaveable()
    {
        SaveLoadManager.Instance.RegisterSaveable(this); ;
    }
    GameSaveData GenerateSaveData();
        void RestoreData(GameSaveData saveData);
}
