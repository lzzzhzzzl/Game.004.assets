using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Strategy.Map;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>, ISaveable
{
    public SceneDetail startScene;
    private CanvasGroup fadeCanvasGroup;
    private string currentSceneName;
    private Vector3 spawnPoint;
    private Vector3 positionToGo;
    private SceneDetail currentScene;
    private bool isFristLoad;
    private Dictionary<string, bool> sceneHasGenerated = new Dictionary<string, bool>();
    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.TransitionInMapEvent += OnTransitionInMapEvent;
        EventHandler.TransitionInIslandEvent += OnTransitionInIslandEvent;
        EventHandler.CloseCurrentScene += OnCloseCurrentScene;
    }
    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.TransitionInMapEvent -= OnTransitionInMapEvent;
        EventHandler.TransitionInIslandEvent -= OnTransitionInIslandEvent;
        EventHandler.CloseCurrentScene -= OnCloseCurrentScene;
    }

    private void OnCloseCurrentScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); // 关闭当前场景
    }
    private void OnEndGameEvent()
    {
        StartCoroutine(UnloadScene());
    }

    private IEnumerator UnloadScene()
    {
        EventHandler.CallBeforeSceneLoadEvent();
        yield return Fade(1f);
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return Fade(0);
    }

    protected override void Awake()
    {
        base.Awake();
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
    }


    /// <summary>
    /// 加载场景并设置激活
    /// </summary>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
    }
    private void OnTransitionInMapEvent(SceneDetail sceneToGo, Vector3 positionToBack)
    {
        this.currentScene.startPos = positionToBack;
        StartCoroutine(TransirionInRandomMap(sceneToGo));
    }
    private void OnTransitionInIslandEvent(SceneDetail sceneDetail)
    {
        currentScene = sceneDetail;
        if (sceneDetail.isRandomMap)
        {
            StartCoroutine(TransirionInRandomMap(sceneDetail));
        }
        else
        {
            StartCoroutine(TransirionInFixedmap(sceneDetail));
        }
    }
    private IEnumerator TransirionInRandomMap(SceneDetail sceneDetail)
    {
        EventHandler.CallIsInCampSceneEvent(sceneDetail.targetScene == "Camp");
        spawnPoint = sceneDetail.startPos;

        if (SceneManager.GetActiveScene().name != "PersistentScene") // 第一次加载时，也就是没有加载游戏场景时，PersistentScene也就是管理场景，所有Manager所在的场景是ActiveScenen，这个不能关闭，否则就全没了
        {
            //呼叫场景加载前的事件，1.保存即将关闭的场景的信息，包含瓦片信息以及地图元素信息（前提是场景存在）。2.暂停一切玩家可进行的操作。3.暂停时间。4.暂停游戏判定。
            EventHandler.CallBeforeSceneLoadEvent();
        }

        //分配进度条加载图片，并初始化进度条
        EventHandler.CallStartSliderEvent(sceneDetail.sprite);
        yield return Fade(1); //展示加载界面

        string sceneName = sceneDetail.targetScene;
        EventHandler.CallSetSliderEvent(0.2f, "加载场景"); // 更新加载进度条的进度


        if (SceneManager.GetActiveScene().name != "PersistentScene") // 第一次加载时，也就是没有加载游戏场景时，PersistentScene也就是管理场景，所有Manager所在的场景是ActiveScenen，这个不能关闭，否则就全没了
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); // 关闭当前场景
        }


        //加载场景
        yield return LoadSceneSetActive(sceneName);
        currentSceneName = sceneDetail.mapID + ":" + sceneDetail.seed; // 当前场景名称，再保存场景是会用到，所以就算要改变，也要在保存后再改变。

        //呼叫场景加载后的事件，1.Manager类更新信息为当前场景的信息，一般为物品存放的GameObject父级的索引
        EventHandler.CallAfterSceneLoadEvent();

        if (isFristLoad)
        {
            EventHandler.CallMoveToPosition(positionToGo);
            isFristLoad = false;
        }
        else
            EventHandler.CallMoveToPosition(sceneDetail.startPos);


        EventHandler.CallSetSliderEvent(0.6f, "生成地图数据"); // 更新加载进度条的进度
        if (sceneName == "MainMap")
            yield return GridMapManager.Instance.GenerateMainMap(currentSceneName, sceneDetail.seed); // 生成瓦片地图时使用了携程，所以需要这样加载，如果不使用携程大量的SetTile操作会导致严重的卡顿
        else if (sceneName == "Building")
            yield return GridMapManager.Instance.GenenrateBuildingInside(currentSceneName, sceneDetail.mapID); // 生成建筑内场景


        EventHandler.CallSetSliderEvent(0.8f, "生成场景元素"); // 更新加载进度条的进度
        if (!sceneHasGenerated.ContainsKey(currentSceneName))
        {
            if (sceneName == "MainMap")
                yield return BlockManager.Instance.OnBuildAllBlock(currentSceneName); // 生成场景元素时同样使用了携程，因为生成场景元素使用Instantiate函数，同样会导致卡顿
            else if (sceneName == "Building")
                yield return BlockManager.Instance.BuildBuildingInsideInventory(sceneDetail.mapID);
            sceneHasGenerated.Add(currentSceneName, true);
        }
        else
        {
            EventHandler.CallLoadSceneDataEvent(); // 地图元素信息保存在不同的Manager类中，需要全部生成
        }

        //完成场景加载
        EventHandler.CallSetSliderEvent(1f, "完成初始化操作");
        EventHandler.CallEndSliderEvent();
        EventHandler.CallAfterSceneDataLoadEvent(); // 完成数据加载事件，角色、判定、时间的锁定解除
        yield return Fade(0f);
    }
    private IEnumerator TransirionInFixedmap(SceneDetail sceneDetail)
    {
        EventHandler.CallIsInCampSceneEvent(sceneDetail.targetScene == "Camp");
        spawnPoint = sceneDetail.startPos;

        if (SceneManager.GetActiveScene().name != "PersistentScene") // 第一次加载时，也就是没有加载游戏场景时，PersistentScene也就是管理场景，所有Manager所在的场景是ActiveScenen，这个不能关闭，否则就全没了
        {
            //呼叫场景加载前的事件，1.保存即将关闭的场景的信息，包含瓦片信息以及地图元素信息（前提是场景存在）。2.暂停一切玩家可进行的操作。3.暂停时间。4.暂停游戏判定。
            EventHandler.CallBeforeSceneLoadEvent();
        }
        EventHandler.CallStartSliderEvent(sceneDetail.sprite);
        yield return Fade(1);

        EventHandler.CallSetSliderEvent(0.2f, "加载场景"); // 更新加载进度条的进度
        if (SceneManager.GetActiveScene().name != "PersistentScene") // 第一次加载时，也就是没有加载游戏场景时，PersistentScene也就是管理场景，所有Manager所在的场景是ActiveScenen，这个不能关闭，否则就全没了
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); // 关闭当前场景
        }


        EventHandler.CallSetSliderEvent(0.4f, "加载场景"); // 更新加载进度条的进度

        string sceneName = sceneDetail.targetScene;
        yield return LoadSceneSetActive(sceneName);
        currentSceneName = sceneDetail.mapID + ":" + sceneDetail.seed; // 当前场景名称，再保存场景是会用到，所以就算要改变，也要在保存后再改变。

        EventHandler.CallAfterSceneLoadEvent();
        if (!sceneHasGenerated.ContainsKey(currentSceneName))
            sceneHasGenerated.Add(currentSceneName, true);
        else
            EventHandler.CallLoadSceneDataEvent();


        EventHandler.CallSetSliderEvent(0.6f, "传送玩家"); // 更新加载进度条的进度
        if (isFristLoad)
        {
            EventHandler.CallMoveToPosition(positionToGo);
            isFristLoad = false;
        }
        else
            EventHandler.CallMoveToPosition(sceneDetail.startPos);


        EventHandler.CallSetSliderEvent(1f, "完成初始化操作"); // 更新加载进度条的进度
        EventHandler.CallEndSliderEvent();
        Debug.Log("当前加载地场景:" + currentSceneName);
        EventHandler.CallAfterSceneDataLoadEvent(); // 完成数据加载事件，角色、判定、时间的锁定解除
        yield return Fade(0);
    }

    public SceneDetail GetCurrentScene()
    {
        return currentScene;
    }
    public string GetCurrentMapID()
    {
        return currentScene.mapID;
    }
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }
    private IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;

        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
        fadeCanvasGroup.blocksRaycasts = false;
    }
    public GameSaveData GenerateSaveData()
    {
        isFristLoad = false;
        GameSaveData saveData = new GameSaveData();
        SceneNameSaved sceneNameSaved = new SceneNameSaved(currentScene.seed, currentScene.mapID);
        saveData.currentScene = sceneNameSaved;
        saveData.sceneHasGenerated = this.sceneHasGenerated;
        saveData.playerPosition = new SerializableVector3(GameObject.FindGameObjectWithTag("Player").transform.position);
        EventHandler.CallBeforeSceneLoadEvent();

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        isFristLoad = true;
        this.currentScene = GridMapManager.Instance.GetSceneDetail(saveData.currentScene);
        positionToGo = saveData.playerPosition.ToVector3();
        this.sceneHasGenerated = saveData.sceneHasGenerated;
        OnTransitionInIslandEvent(currentScene);
    }

    private void OnStartNewGameEvent(int index)
    {
        isFristLoad = false;
        this.currentScene = startScene;
        this.sceneHasGenerated.Clear();
        OnTransitionInIslandEvent(currentScene);
    }
}
