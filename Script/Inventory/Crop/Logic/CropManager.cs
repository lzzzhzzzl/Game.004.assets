using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : Singleton<CropManager>, ISaveable
{
    private Transform cropParent;
    private Dictionary<string, List<ScenenCrop>> sceneCropDict = new Dictionary<string, List<ScenenCrop>>();
    public string GUID => GetComponent<DataGUID>().guid;
    private void OnEnable()
    {
        EventHandler.GenerateCropEvent += OnGenerateCropEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.LoadSceneDataEvent += OnLoadSceneDataEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.GenerateCropEvent -= OnGenerateCropEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
        EventHandler.LoadSceneDataEvent -= OnLoadSceneDataEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnBeforeSceneLoadEvent()
    {
        SaveSceneCrop();
    }
    private void OnAfterSceneLoadEvent()
    {
        cropParent = GameObject.FindWithTag("CropParent").transform;

        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneCropDict.ContainsKey(sceneName))
            return;
        for (int i = 0; i < cropParent.childCount; i++)
        {
            Destroy(cropParent.GetChild(i).gameObject);
        }
    }
    private void OnLoadSceneDataEvent()
    {
        LoadSceneCrop();
    }


    private void OnGenerateCropEvent(string cropID, Vector3 pos)
    {
        CropDetail cropDetail = InventoryManager.Instance.GetCropDetail(cropID);
        if (cropDetail != null)
        {
            GameObject crop = Instantiate(cropDetail.cropPrefab, pos, Quaternion.identity, cropParent);
        }
    }



    private void SaveSceneCrop()
    {
        List<ScenenCrop> sceneCropList = new List<ScenenCrop>();
        for (int i = 0; i < cropParent.childCount; i++)
        {
            Crop crop;
            if (cropParent.GetChild(i).TryGetComponent<Crop>(out crop))
            {
                ScenenCrop scenenCrop = new ScenenCrop();
                scenenCrop.cropID = crop.cropID;
                scenenCrop.position = new SerializableVector3(cropParent.GetChild(i).position);

                sceneCropList.Add(scenenCrop);
            }
        }
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneCropDict.ContainsKey(sceneName))
            sceneCropDict.Add(sceneName, sceneCropList);
        else
            sceneCropDict[sceneName] = sceneCropList;
        Debug.Log("------场景:" + sceneName + "   （作物）保存: " + cropParent.childCount + " 个组件------");
    }
    private void LoadSceneCrop()
    {
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneCropDict.ContainsKey(sceneName))
            return;


        // Debug.Log(cropParent.childCount);
        List<ScenenCrop> sceneCropList = sceneCropDict[sceneName];

        for (int i = 0; i < sceneCropList.Count; i++)
        {
            ScenenCrop scenenCrop = sceneCropList[i];

            CropDetail cropDetail = InventoryManager.Instance.GetCropDetail(scenenCrop.cropID);
            if (cropDetail != null)
            {
                Vector3 pos = scenenCrop.position.ToVector3();
                GameObject building = Instantiate(cropDetail.cropPrefab, pos, Quaternion.identity, cropParent);
            }
        }
        Debug.Log("------场景:" + sceneName + "   （作物）加载: " + cropParent.childCount + " 个组件------");
    }


    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneCropDict = this.sceneCropDict;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.sceneCropDict = saveData.sceneCropDict;
    }
    private void OnStartNewGameEvent(int index)
    {
        sceneCropDict.Clear();
    }
}
