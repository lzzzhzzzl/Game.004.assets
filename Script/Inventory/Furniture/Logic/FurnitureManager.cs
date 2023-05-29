using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureManager : Singleton<FurnitureManager>, ISaveable
{
    private Transform furnitureParent;
    private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();
    private Dictionary<string, int> sceneFurnitureIndexDict = new Dictionary<string, int>();
    public string GUID => GetComponent<DataGUID>().guid;
    private void OnEnable()
    {
        EventHandler.GenerateFurnitureEvent += OnGenerateFurnitureEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.LoadSceneDataEvent += OnLoadSceneDataEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.GenerateFurnitureEvent -= OnGenerateFurnitureEvent;
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
        SaveSceneFurniture();
    }
    private void OnAfterSceneLoadEvent()
    {
        furnitureParent = GameObject.FindWithTag("FurnitureParent").transform;

        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneFurnitureDict.ContainsKey(sceneName))
            return;
        for (int i = 0; i < furnitureParent.childCount; i++)
        {
            Destroy(furnitureParent.GetChild(i).gameObject);
        }
    }
    private void OnLoadSceneDataEvent()
    {
        LoadSceneFurniture();
    }


    private void OnGenerateFurnitureEvent(BluePrintDetail bluePrintDetail, Vector3 pos)
    {
        furnitureParent = GameObject.FindWithTag("FurnitureParent").transform;
        if (bluePrintDetail != null)
        {
            GameObject furniture = Instantiate(bluePrintDetail.furnitureBuilding, pos, Quaternion.identity, furnitureParent);
            if (furniture.transform.GetChild(0).GetComponent<IBox>() != null)
            {
                furniture.transform.GetChild(0).GetComponent<IBox>().InitBox(GetNewFurnitureIndex(bluePrintDetail.furnitureID));
            }
        }
    }
    private int GetNewFurnitureIndex(string furnitureID)
    {
        if (sceneFurnitureIndexDict.ContainsKey(furnitureID))
        {
            return ++sceneFurnitureIndexDict[furnitureID];
        }
        else
        {
            sceneFurnitureIndexDict.Add(furnitureID, 0);
            return ++sceneFurnitureIndexDict[furnitureID];
        }
    }
    private void SaveSceneFurniture()
    {
        List<SceneFurniture> sceneFurnitureList = new List<SceneFurniture>();
        for (int i = 0; i < furnitureParent.childCount; i++)
        {
            Furniture furniture;
            if (furnitureParent.GetChild(i).TryGetComponent<Furniture>(out furniture))
            {
                SceneFurniture sceneFurniture = new SceneFurniture();
                sceneFurniture.furnitureID = furniture.furnitureID;
                sceneFurniture.position = new SerializableVector3(furnitureParent.GetChild(i).position);
                if (furniture.transform.GetChild(0).GetComponent<IBox>() != null)
                {
                    sceneFurniture.index = furniture.transform.GetChild(0).GetComponent<IBox>().GetIndex();
                }
                sceneFurnitureList.Add(sceneFurniture);
            }
        }
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneFurnitureDict.ContainsKey(sceneName))
            sceneFurnitureDict.Add(sceneName, sceneFurnitureList);
        else
            sceneFurnitureDict[sceneName] = sceneFurnitureList;

        Debug.Log("------场景:" + sceneName + "   （家具）保存: " + furnitureParent.childCount + " 个组件------");
    }
    private void LoadSceneFurniture()
    {
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneFurnitureDict.ContainsKey(sceneName))
            return;


        List<SceneFurniture> sceneFurnitureList = sceneFurnitureDict[sceneName];

        for (int i = 0; i < sceneFurnitureList.Count; i++)
        {
            SceneFurniture sceneFurniture = sceneFurnitureList[i];

            BluePrintDetail bluePrintDetail = InventoryManager.Instance.GetBluePrintDetail(sceneFurniture.furnitureID);
            if (bluePrintDetail != null)
            {
                Vector3 pos = sceneFurniture.position.ToVector3();
                GameObject furniture = Instantiate(bluePrintDetail.furnitureBuilding, pos, Quaternion.identity, furnitureParent);
                if (furniture.transform.GetChild(0).GetComponent<IBox>() != null)
                {
                    furniture.transform.GetChild(0).GetComponent<IBox>().InitBox(sceneFurniture.index);
                }

            }
        }
        Debug.Log("------场景:" + sceneName + "   （家具）加载: " + furnitureParent.childCount + " 个组件------");
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneFurnitureDict = this.sceneFurnitureDict;
        saveData.sceneFurnitureIndexDict = this.sceneFurnitureIndexDict;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.sceneFurnitureDict = saveData.sceneFurnitureDict;
        this.sceneFurnitureIndexDict = saveData.sceneFurnitureIndexDict;
    }
    private void OnStartNewGameEvent(int index)
    {
        sceneFurnitureDict.Clear();
        sceneFurnitureIndexDict.Clear();
    }
}
