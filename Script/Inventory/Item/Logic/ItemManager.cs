using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Strategy.Map;

public class ItemManager : MonoBehaviour, ISaveable
{
    /// <summary>
    /// 在地图生成物品的预制体
    /// </summary>
    public Item itemPrefer;

    /// <summary>
    /// 在地图生成可抛出物品的预制体
    /// </summary>
    public Item bounceItemPrefer;
    private Transform itemParent;
    private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.GenerateItemEvent += OnGenerateItemEvent;
        EventHandler.LoadSceneDataEvent += OnLoadSceneDataEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
        EventHandler.GenerateItemEvent -= OnGenerateItemEvent;
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
        SaveSceneItem();

        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneItemDict.ContainsKey(sceneName))
            return;
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }
    }
    private void SaveSceneItem()
    {
        List<SceneItem> sceneItemList = new List<SceneItem>();
        for (int i = 0; i < itemParent.childCount; i++)
        {
            Item item;
            if (itemParent.GetChild(i).TryGetComponent<Item>(out item))
            {
                SceneItem sceneItem = new SceneItem();
                sceneItem.ItemID = item.itemID;
                sceneItem.position = new SerializableVector3(itemParent.GetChild(i).position);
                sceneItemList.Add(sceneItem);
            }
        }
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneItemDict.ContainsKey(sceneName))
            sceneItemDict.Add(sceneName, sceneItemList);
        else
            sceneItemDict[sceneName] = sceneItemList;
        Debug.Log("------场景:" + sceneName + "   （物品）保存: " + itemParent.childCount + " 个组件------");
    }
    private void OnLoadSceneDataEvent()
    {
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!sceneItemDict.ContainsKey(sceneName))
            return;

        List<SceneItem> sceneItemList = sceneItemDict[sceneName];

        for (int i = 0; i < sceneItemList.Count; i++)
        {
            SceneItem sceneItem = sceneItemList[i];

            ItemDetail itemDetail = InventoryManager.Instance.GetItemDetail(sceneItem.ItemID);
            if (itemDetail != null)
            {
                Vector3 pos = sceneItem.position.ToVector3();
                Item item = Instantiate(itemPrefer, pos, Quaternion.identity, itemParent);
                item.Init(itemDetail.itemID);
            }
        }
        Debug.Log("------场景:" + sceneName + "   （物品）加载: " + itemParent.childCount + " 个组件------");
    }

    private void OnAfterSceneLoadEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
    }
    private void OnGenerateItemEvent(string itemID, Vector3 pos)
    {
        var item = Instantiate(bounceItemPrefer, pos, Quaternion.identity, itemParent);
        item.itemID = itemID;
        item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.sceneItemDict = this.sceneItemDict;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.sceneItemDict = saveData.sceneItemDict;
    }
    private void OnStartNewGameEvent(int index)
    {
        sceneItemDict.Clear();
    }
}
