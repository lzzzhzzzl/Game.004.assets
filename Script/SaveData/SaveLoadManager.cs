using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private List<ISaveable> saveableList = new List<ISaveable>();
    public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[2]);
    private string jsonFolder;
    private int currentDataIndex;
    protected override void Awake()
    {
        base.Awake();
        jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
        ReadSaveData();
    }
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.ReLoadGameEvent += OnReLoadGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.ReLoadGameEvent -= OnReLoadGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnStartNewGameEvent(int index)
    {
        currentDataIndex = index;
    }

    private void OnReLoadGameEvent()
    {
        if (dataSlots[currentDataIndex] != null)
            Load(currentDataIndex);
        else
            EventHandler.CallStartNewGameEvent(currentDataIndex);
    }

    private void OnEndGameEvent()
    {
        Save(currentDataIndex);
    }


    private void Save(int index)
    {
        DataSlot data = new DataSlot();

        foreach (var saveable in saveableList)
        {
            data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
        }
        Debug.Log("保存为" + index);
        dataSlots[index] = data;

        var resultPath = jsonFolder + "data" + index + ".json";

        var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);

        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        Debug.Log("DATA" + index + "SAVED!");

        File.WriteAllText(resultPath, jsonData);
        Debug.Log(resultPath);
    }

    public void Load(int index)
    {
        currentDataIndex = index;
        var resultPath = jsonFolder + "data" + index + ".json";

        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

        foreach (var saveable in saveableList)
        {
            if (saveable.GUID != TransitionManager.Instance.GUID)
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
        }
        saveableList.Find(i => i.GUID == TransitionManager.Instance.GUID).RestoreData(jsonData.dataDict[TransitionManager.Instance.GUID]);
    }

    private void ReadSaveData()
    {
        if (Directory.Exists(jsonFolder))
        {
            for (int i = 0; i < dataSlots.Count; i++)
            {
                var resultPath = jsonFolder + "data" + i + ".json";
                Debug.Log(resultPath);
                if (File.Exists(resultPath))
                {
                    var stringData = File.ReadAllText(resultPath);
                    var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                    dataSlots[i] = jsonData;
                }
            }
        }
    }
    public void DeleteGameData(int index)
    {
        SaveLoadManager.Instance.dataSlots[index] = null;
        var resultPath = jsonFolder + "data" + index + ".json";
        if (File.Exists(resultPath))
        {
            File.Delete(resultPath);
        }
    }
    public void RegisterSaveable(ISaveable saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

}
