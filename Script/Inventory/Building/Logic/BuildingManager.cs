using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Strategy.Map;

public class BuildingManager : Singleton<BuildingManager>, ISaveable
{
    private Transform buildingParent;
    private Dictionary<string, List<SceneBuilding>> sceneBuildingDict = new Dictionary<string, List<SceneBuilding>>();

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.GenerateExteriorBuildingEvent += OnGenerateExteriorBuildingEvent;
        EventHandler.GenerateInsideBuildingEvent += OnGenerateInsideBuildingEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.LoadSceneDataEvent += OnLoadSceneDataEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.GenerateExteriorBuildingEvent -= OnGenerateExteriorBuildingEvent;
        EventHandler.GenerateInsideBuildingEvent -= OnGenerateInsideBuildingEvent;
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
        SaveSceneBuilding();
    }
    private void OnAfterSceneLoadEvent()
    {
        buildingParent = GameObject.FindWithTag("BuildingParent").transform;

        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneBuildingDict.ContainsKey(sceneName))
            return;
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            Destroy(buildingParent.GetChild(i).gameObject);
        }
    }
    private void OnLoadSceneDataEvent()
    {
        LoadSceneBuilding();
    }

    private void OnGenerateExteriorBuildingEvent(string buildingID, Vector3 pos, string blockID)
    {
        BuildingExteriorDetail buildingDetail = InventoryManager.Instance.GetBuildingExteriorDetail(buildingID);
        if (buildingDetail != null)
        {
            GameObject building = Instantiate(buildingDetail.buildingPrefab, pos, Quaternion.identity, buildingParent);

            if (buildingDetail.buildingType == BuildingType.Building)
            {
                Teleport teleport;
                if (building.transform.GetChild(0).TryGetComponent<Teleport>(out teleport))
                {
                    int seed = InventoryManager.Instance.GetBuildingNum(buildingID);
                    if (seed != -1)
                    {
                        teleport.sceneToGo.seed = seed;
                        teleport.sceneToGo.mapID = buildingID;
                        teleport.sceneToGo.sprite = buildingDetail.sprite;
                    }
                }
            }
        }
    }

    private void OnGenerateInsideBuildingEvent(string buildingID, Vector3 pos)
    {
        BuildingInsideDetail buildingDetail = InventoryManager.Instance.GetBuildingInsideDetail(buildingID);
        if (buildingDetail != null)
        {
            GameObject building = Instantiate(buildingDetail.buildingPrefab, pos, Quaternion.identity, buildingParent);

            Teleport teleport;
            if (building.transform.GetChild(0).TryGetComponent<Teleport>(out teleport))
            {
                SceneDetail sceneToBack = TransitionManager.Instance.GetCurrentScene();
                teleport.sceneToGo.seed = sceneToBack.seed;
                teleport.sceneToGo.mapID = sceneToBack.mapID;
                teleport.sceneToGo.sprite = sceneToBack.sprite;
                teleport.sceneToGo.targetScene = sceneToBack.targetScene;
                teleport.sceneToGo.startPos = sceneToBack.startPos;

            }
        }
    }

    private void SaveSceneBuilding()
    {
        List<SceneBuilding> sceneBuildingList = new List<SceneBuilding>();
        for (int i = 0; i < buildingParent.childCount; i++)
        {
            Building building;
            if (buildingParent.GetChild(i).TryGetComponent<Building>(out building))
            {
                SceneBuilding sceneBuilding = new SceneBuilding();
                sceneBuilding.buildingID = building.buildingID;
                sceneBuilding.position = new SerializableVector3(buildingParent.GetChild(i).position);

                Teleport teleport;
                if (buildingParent.GetChild(i).GetChild(0).TryGetComponent<Teleport>(out teleport))
                {
                    sceneBuilding.teleportMapID = teleport.sceneToGo.mapID;
                    sceneBuilding.teleportSeed = teleport.sceneToGo.seed;
                    sceneBuilding.teleportSceneName = teleport.sceneToGo.targetScene;
                    sceneBuilding.targetPosition = new SerializableVector3(teleport.sceneToGo.startPos);

                    Debug.Log("保存传送坐标: 目标ID:" + sceneBuilding.teleportMapID + "  目标种子:" + sceneBuilding.teleportSeed + "  目标场景:" + sceneBuilding.teleportSceneName);
                }
                sceneBuildingList.Add(sceneBuilding);
            }
        }
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneBuildingDict.ContainsKey(sceneName))
            sceneBuildingDict.Add(sceneName, sceneBuildingList);
        else
            sceneBuildingDict[sceneName] = sceneBuildingList;
        Debug.Log("------场景:" + sceneName + "   （建筑）保存: " + buildingParent.childCount + " 个组件------");
    }

    private void LoadSceneBuilding()
    {
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneBuildingDict.ContainsKey(sceneName))
            return;

        List<SceneBuilding> sceneBuildingList = sceneBuildingDict[sceneName];

        for (int i = 0; i < sceneBuildingList.Count; i++)
        {
            SceneBuilding sceneBuilding = sceneBuildingList[i];
            BuildingExteriorDetail buildingDetail = InventoryManager.Instance.GetBuildingExteriorDetail(sceneBuilding.buildingID);
            if (buildingDetail != null)
            {
                Vector3 pos = sceneBuilding.position.ToVector3();
                GameObject building = Instantiate(buildingDetail.buildingPrefab, pos, Quaternion.identity, buildingParent);
                Teleport teleport;
                if (building.transform.GetChild(0).TryGetComponent<Teleport>(out teleport))
                {
                    teleport.sceneToGo.seed = sceneBuilding.teleportSeed;
                    teleport.sceneToGo.mapID = sceneBuilding.teleportMapID;
                    teleport.sceneToGo.targetScene = sceneBuilding.teleportSceneName;
                    teleport.sceneToGo.startPos = sceneBuilding.targetPosition.ToVector3();
                    teleport.sceneToGo.sprite = buildingDetail.sprite;

                    Debug.Log("加载传送坐标: 目标ID:" + sceneBuilding.teleportMapID + "  目标种子:" + sceneBuilding.teleportSeed + "  目标场景:" + sceneBuilding.teleportSceneName);
                }
            }
        }
        Debug.Log("------场景:" + sceneName + "   （建筑）加载: " + buildingParent.childCount + " 个组件------");
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneBuildingDict = this.sceneBuildingDict;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.sceneBuildingDict = saveData.sceneBuildingDict;
    }

    private void OnStartNewGameEvent(int index)
    {
        sceneBuildingDict.Clear();
    }
}
