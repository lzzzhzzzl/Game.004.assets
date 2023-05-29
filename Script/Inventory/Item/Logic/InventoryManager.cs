using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>, ISaveable
{
    [Header("物品的信息")]
    public ItemDetails_SO itemDetails_SO;
    [Header("可采集类物品信息")]
    public CropDetailData_SO cropDetailData_SO;
    [Header("建筑信息")]
    public BuildingExteriorDetailsData_SO buildingExteriorDetailsData_SO;
    public BuildingInsideDetailData_SO buildingInsideDetailsData_SO;
    [Header("背包数据")]
    public InventorySlotData_SO equipmentBag;
    public InventorySlotData_SO equipmentBagTemp;
    public InventorySlotData_SO playerBag;
    public InventorySlotData_SO playerBagTemp;
    public InventorySlotData_SO currentBoxData;
    [Header("蓝图数据")]
    public BulePrintData_SO bulePrintData_SO;
    private Dictionary<string, List<InventoryItem>> boxItemList = new Dictionary<string, List<InventoryItem>>();
    private Dictionary<string, int> buildingIDnumberDict = new Dictionary<string, int>();
    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        EventHandler.ReduceItemDurability += OnReduceItemDurability;
        EventHandler.GenerateInventoryItemEvent += OnGenerateInventoryItem;
        EventHandler.mouseClickBulePrintEvent += OnmouseClickBulePrintEvent;
        EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.RemoveFurnitureEvent += OnRemoveFurnitureEvent;
    }

    private void OnDisable()
    {
        EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        EventHandler.ReduceItemDurability -= OnReduceItemDurability;
        EventHandler.GenerateInventoryItemEvent -= OnGenerateInventoryItem;
        EventHandler.mouseClickBulePrintEvent -= OnmouseClickBulePrintEvent;
        EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.RemoveFurnitureEvent -= OnRemoveFurnitureEvent;
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    /// <summary>
    /// 根据ID获取物品信息
    /// </summary>
    public ItemDetail GetItemDetail(string itemID)
    {
        return itemDetails_SO.itemDetailList.Find(i => i.itemID == itemID);
    }
    /// <summary>
    /// 根据ID获取可采集的信息
    /// </summary>
    public CropDetail GetCropDetail(string cropID)
    {
        return cropDetailData_SO.cropDetailList.Find(i => i.cropID == cropID);
    }
    public BluePrintDetail GetBluePrintDetail(string furnitureID)
    {
        return bulePrintData_SO.GetBluePrintDetails(furnitureID);
    }
    private void OnRemoveFurnitureEvent(Transform furnitureTransform, BluePrintDetail bluePrintDetail)
    {
        Vector3 offest;
        Vector3 position = furnitureTransform.position + Vector3.right;
        Destroy(furnitureTransform.gameObject);
        foreach (var resourceItem in bluePrintDetail.resourceItem)
        {
            for (int i = 0; i < resourceItem.itemAmount / 2; i++)
            {
                offest = new Vector3(Random.Range(0, (float)bluePrintDetail.size.x), Random.Range(0, (float)bluePrintDetail.size.y));
                EventHandler.CallGenerateItemEvent(resourceItem.itemID, position + offest);
            }
        }

    }
    /// <summary>
    /// 根据ID获取建筑物品的信息，这是建筑的外部信息，可以说是全部的建筑信息，但有一些建筑是可以进入的，这些建筑内部的信息单独保存(因为信息有些区别)
    /// </summary>
    public BuildingExteriorDetail GetBuildingExteriorDetail(string buildingID)
    {
        return buildingExteriorDetailsData_SO.buildingExteriorList.Find(i => i.buildingID == buildingID);
    }
    public BuildingInsideDetail GetBuildingInsideDetail(string buildingID)
    {
        return buildingInsideDetailsData_SO.buildingInsideList.Find(i => i.buildingID == buildingID);
    }


    public int GetBuildingNum(string buildingID)
    {
        //TAG:这里我要他在MainMap场景才会分配建筑ID（因为可能出现同一个建筑但是地图上有好多个，每个建筑内部是不一样的），如果希望在其他的某个场景也会分配这样的建筑ID，则需要在这里添加
        if (buildingExteriorDetailsData_SO.buildingExteriorList.Find(i => i.buildingID == buildingID) != null && SceneManager.GetActiveScene().name == "MainMap")
        {
            if (buildingIDnumberDict.ContainsKey(buildingID))
            {
                return ++buildingIDnumberDict[buildingID];
            }
            else
            {
                buildingIDnumberDict.Add(buildingID, 0);
                return ++buildingIDnumberDict[buildingID];
            }
        }
        return -1;
    }
    private void OnGenerateInventoryItem(string inventoryID, Vector3 pos, string blockID)
    {
        string[] inventory = inventoryID.Split("-");
        switch (inventory[0])
        {
            case "Item":
                EventHandler.CallGenerateItemEvent(inventoryID, pos);
                break;
            case "Crop":
                EventHandler.CallGenerateCropEvent(inventoryID, pos);
                break;
            case "Building":
                BuildingExteriorDetail buildingItem = InventoryManager.Instance.GetBuildingExteriorDetail(inventoryID);
                if (buildingItem != null)
                {
                    if (blockID != "null")
                        EventHandler.CallGenerateExteriorBuildingEvent(inventoryID, pos, blockID);
                    else
                        EventHandler.CallGenerateInsideBuildingEvent(inventoryID, pos);
                }
                break;
            case "Furniture":
                break;
        }
    }

    /// <summary>
    /// 交换物品
    /// </summary>
    public void SwapItem(int fromIndex, int targetIndex)
    {
        InventoryItem currentItem = playerBag.itemList[fromIndex];
        InventoryItem targetItem = playerBag.itemList[targetIndex];

        if (fromIndex != targetIndex)
        {
            if (targetItem.itemID != "" && currentItem.itemID != targetItem.itemID)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else if (currentItem.itemID == targetItem.itemID)
            {
                ItemDetail item = GetItemDetail(currentItem.itemID);
                if (item != null)
                {
                    if (item.itemType != ItemType.AxeTool && item.itemType != ItemType.PickAxe && item.itemType != ItemType.Sword && item.itemType != ItemType.Throw)
                    {
                        if (targetItem.itemAmount + currentItem.itemAmount <= Settings.itemMaxNumber)
                        {
                            targetItem.itemAmount += currentItem.itemAmount;
                            playerBag.itemList[targetIndex] = targetItem;
                            playerBag.itemList[fromIndex] = new InventoryItem();
                        }
                        else
                        {
                            int count = targetItem.itemAmount + currentItem.itemAmount - Settings.itemMaxNumber;
                            targetItem.itemAmount = Settings.itemMaxNumber;
                            currentItem.itemAmount = count;
                            playerBag.itemList[targetIndex] = targetItem;
                            playerBag.itemList[fromIndex] = currentItem;
                        }
                    }
                    else
                    {
                        playerBag.itemList[fromIndex] = targetItem;
                        playerBag.itemList[targetIndex] = currentItem;
                    }
                }
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
        }
        //更新背包
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
    }
    public void SwapItem(int fromIndex, InventoryLocation fromLocation, int targetIndex, InventoryLocation targetLocation)
    {
        List<InventoryItem> fromItemList = GetItemList(fromLocation);
        List<InventoryItem> targetItemList = GetItemList(targetLocation);

        InventoryItem currentItem = fromItemList[fromIndex];

        if (targetIndex < targetItemList.Count && fromIndex != targetIndex)
        {
            InventoryItem targetItem = targetItemList[targetIndex];

            if (targetItem.itemID != "" && currentItem.itemID != targetItem.itemID)
            {
                fromItemList[fromIndex] = targetItem;
                targetItemList[targetIndex] = currentItem;
            }
            else if (currentItem.itemID == targetItem.itemID)
            {
                ItemDetail item = GetItemDetail(currentItem.itemID);
                if (item != null)
                {
                    if (item.itemType != ItemType.AxeTool && item.itemType != ItemType.PickAxe && item.itemType != ItemType.Sword && item.itemType != ItemType.Throw)
                    {
                        if (targetItem.itemAmount + currentItem.itemAmount <= Settings.itemMaxNumber)
                        {
                            targetItem.itemAmount += currentItem.itemAmount;
                            targetItemList[targetIndex] = targetItem;
                            fromItemList[fromIndex] = new InventoryItem();
                        }
                        else
                        {
                            int count = targetItem.itemAmount + currentItem.itemAmount - Settings.itemMaxNumber;
                            targetItem.itemAmount = Settings.itemMaxNumber;
                            currentItem.itemAmount = count;
                            targetItemList[targetIndex] = targetItem;
                            fromItemList[fromIndex] = currentItem;
                        }
                    }
                    else
                    {
                        fromItemList[fromIndex] = targetItem;
                        targetItemList[targetIndex] = currentItem;
                    }
                }

            }
            else //目标格子为空
            {
                targetItemList[targetIndex] = currentItem;
                fromItemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUIEvent(fromLocation, fromItemList);
            EventHandler.CallUpdateInventoryUIEvent(targetLocation, targetItemList);
        }
    }
    private List<InventoryItem> GetItemList(InventoryLocation location)
    {
        return location switch
        {
            InventoryLocation.Player => playerBag.itemList,
            InventoryLocation.Box => currentBoxData.itemList,
            _ => null
        };
    }

    public void SwapItemWithEquipment(int fromIndex, int targetIndex, ItemDetail itemDetail)
    {
        InventoryItem currentItem = playerBag.itemList[fromIndex];
        InventoryItem targetItem = equipmentBag.itemList[targetIndex];
        if (targetItem.itemID != "")
        {
            playerBag.itemList[fromIndex] = targetItem;
            equipmentBag.itemList[targetIndex] = currentItem;
        }
        else
        {
            playerBag.itemList[fromIndex] = new InventoryItem();
            equipmentBag.itemList[targetIndex] = currentItem;
        }
        EventHandler.CallChangePlayerEquiment(itemDetail);
        EventHandler.CallChangeCharacterEquiment(itemDetail, targetIndex);

        //更新背包
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Equipment, equipmentBag.itemList);
        EventHandler.CallChangeCharacterAnimatorEvent();
        EventHandler.CallSwitchAnimation(PlayerPartType.Hurt);
    }
    public void SwapEquipmentWithItem(int fromIndex, int targetIndex, ItemDetail itemDetail)
    {
        InventoryItem currentItem = equipmentBag.itemList[fromIndex];
        InventoryItem targetItem = playerBag.itemList[targetIndex];
        if (targetItem.itemID != "")
        {
            playerBag.itemList[targetIndex] = currentItem;
            equipmentBag.itemList[fromIndex] = targetItem;
        }
        else
        {
            playerBag.itemList[targetIndex] = currentItem;
            equipmentBag.itemList[fromIndex] = new InventoryItem();
        }

        if (targetItem.itemID != "")
        {
            EventHandler.CallChangePlayerEquiment(itemDetail);
        }
        else
        {
            EventHandler.CallChangePlayerEquiment(null);
        }
        EventHandler.CallChangeCharacterEquiment(itemDetail, fromIndex);

        //更新背包
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Equipment, equipmentBag.itemList);
        EventHandler.CallChangeCharacterAnimatorEvent();
        EventHandler.CallSwitchAnimation(PlayerPartType.Hurt);
    }
    public void AddItem(Item item, bool toDestroy)
    {
        var index = GetItemIndexInBag(item.itemID);

        //世界地图中拾取
        AddItemAtIndex(item.itemID, index, 1);
        if (toDestroy && index != 1)
        {
            Destroy(item.gameObject);
        }

        //更新背包信息
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
    }
    private int GetItemIndexInBag(string ID)
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == ID && playerBag.itemList[i].itemAmount < Settings.itemMaxNumber)
                return i;
        }
        return -1;
    }
    /// <summary>n
    /// 将物品放入指定位置
    /// </summary>
    private void AddItemAtIndex(string ID, int index, int amount)
    {
        if (index == -1 && CheckBagCapacity())    // 背包中不存在该物品
        {
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == "")
                {
                    playerBag.itemList[i] = item;
                    break;
                }
            }
        }
        else    // 背包中存在该物品
        {
            int currentAmount = playerBag.itemList[index].itemAmount + amount;
            if (currentAmount <= Settings.itemMaxNumber)
            {
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
            else
            {
                var item = new InventoryItem { itemID = ID, itemAmount = Settings.itemMaxNumber };
                playerBag.itemList[index] = item;
            }
        }
    }
    /// <summary>
    /// 背包有空地
    /// </summary>
    private bool CheckBagCapacity()
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == "")
                return true;
        }
        return false;
    }

    private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetail itemDetail, int index)
    {
        switch (itemDetail.itemType)
        {
            case ItemType.AxeTool:
            case ItemType.PickAxe:
                EventHandler.CallPlaySoundEvent(SoundName.Axe);
                Crop currentCrop = GetDigGround(mouseWorldPos);
                if (currentCrop != null)
                {
                    currentCrop.ProcessToolAction(itemDetail);
                    OnReduceItemDurability(InventoryLocation.Player, index);
                }
                break;
            case ItemType.Sword:
                playerMeleeAttack(mouseWorldPos, itemDetail, index);
                break;
            case ItemType.Consumable:
                CoverPlayer(itemDetail);
                break;
            default:
                break;
        }
    }
    public void CoverPlayer(ItemDetail itemDetail)
    {
        PlayerParameter player = GameObject.FindWithTag("Player").GetComponent<PlayerParameter>();
        player.CoverState(itemDetail.damage);
        RemoveItemAtplayerBag(itemDetail.itemID, 1);
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
    }
    /// <summary>
    /// 根据碰撞体返回点击位置的农作物
    /// </summary>
    public Crop GetDigGround(Vector3 mouseWorldPos)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
        Crop currentCrop = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Crop>())
            {
                currentCrop = colliders[i].GetComponent<Crop>();
            }
        }
        return currentCrop;
    }
    public void playerMeleeAttack(Vector3 mouseWorldPos, ItemDetail itemDetail, int index)
    {
        if (itemDetail != null)
        {
            Collider2D[] colliders = new Collider2D[20];
            int count = Physics2D.OverlapCircleNonAlloc(mouseWorldPos, (float)itemDetail.itemUseRadius / 2f, colliders);
            for (int i = 0; i < count; i++)
            {
                if (colliders[i].GetComponent<EnemyBaseController>())
                {
                    Transform playerTransform = GameObject.FindWithTag("Player").transform;
                    colliders[i].GetComponent<EnemyBaseController>().HurtState(itemDetail.damage, playerTransform);
                    OnReduceItemDurability(InventoryLocation.Player, index);
                }
                else if (colliders[i].GetComponent<AnimalBaseController>())
                {
                    Transform playerTransform = GameObject.FindWithTag("Player").transform;
                    colliders[i].GetComponent<AnimalBaseController>().HurtState(itemDetail.damage, playerTransform);
                    OnReduceItemDurability(InventoryLocation.Player, index);
                }
                else if (colliders[i].GetComponent<Crop>())
                {
                    Crop currentCrop = colliders[i].GetComponent<Crop>();
                    if (currentCrop != null)
                    {
                        if (currentCrop.ProcessToolAction(itemDetail))
                            OnReduceItemDurability(InventoryLocation.Player, index);
                    }
                }
            }
        }
    }
    public void OnReduceItemDurability(InventoryLocation inventoryLocation, int index)
    {
        ItemDetail itemDetail;
        switch (inventoryLocation)
        {
            case InventoryLocation.Player:
                itemDetail = GetItemDetail(playerBag.itemList[index].itemID);
                if (itemDetail != null)
                {
                    InventoryItem inventoryItem = new InventoryItem()
                    {
                        itemID = playerBag.itemList[index].itemID,
                        itemAmount = playerBag.itemList[index].itemAmount,
                        itemUseTimes = playerBag.itemList[index].itemUseTimes - 1
                    };
                    playerBag.itemList[index] = inventoryItem;
                    if (playerBag.itemList[index].itemUseTimes <= 0)
                        playerBag.itemList[index] = new InventoryItem();
                    EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
                }
                break;
            case InventoryLocation.Equipment:
                itemDetail = GetItemDetail(equipmentBag.itemList[index].itemID);
                if (itemDetail != null)
                {
                    InventoryItem inventoryItem = new InventoryItem()
                    {
                        itemID = equipmentBag.itemList[index].itemID,
                        itemAmount = equipmentBag.itemList[index].itemAmount,
                        itemUseTimes = equipmentBag.itemList[index].itemUseTimes - 1
                    };
                    equipmentBag.itemList[index] = inventoryItem;
                    if (equipmentBag.itemList[index].itemUseTimes <= 0)
                        equipmentBag.itemList[index] = new InventoryItem();
                    EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Equipment, equipmentBag.itemList);
                }
                break;
        }
    }

    public List<BluePrintDetail> GetBulePrintDataList()
    {
        return bulePrintData_SO.bluePrintDataList;
    }
    private void OnmouseClickBulePrintEvent(BluePrintDetail bluePrintDetail, Vector3Int pos)
    {
        for (int i = 0; i < bluePrintDetail.resourceItem.Length; i++)
            RemoveItemAtplayerBag(bluePrintDetail.resourceItem[i].itemID, bluePrintDetail.resourceItem[i].itemAmount);
        EventHandler.CallGenerateFurnitureEvent(bluePrintDetail, pos);
        EventHandler.CallUpdateLightControl();
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
    }


    public List<InventoryItem> GetBoxDataList(int index)
    {
        string key = SceneManager.GetActiveScene().name + "-Box" + index;
        List<InventoryItem> itemList;
        if (boxItemList.TryGetValue(key, out itemList))
        {
            return itemList;
        }
        return null;
    }

    public void AddBoxDataList(Box box)
    {
        string key = SceneManager.GetActiveScene().name + "-Box" + box.GetIndex();
        if (!boxItemList.ContainsKey(key))
        {
            boxItemList.Add(key, box.boxBagData.itemList);
        }
    }

    public List<InventoryItem> GetBoxFixedDataList(int index)
    {
        string key = SceneManager.GetActiveScene().name + "-BoxFixed" + index;
        List<InventoryItem> itemList;
        if (boxItemList.TryGetValue(key, out itemList))
        {
            return itemList;
        }
        return null;
    }
    public void AddBoxDataList(BoxFixed box)
    {
        string key = SceneManager.GetActiveScene().name + "-BoxFixed" + box.GetIndex();
        if (!boxItemList.ContainsKey(key))
        {
            boxItemList.Add(key, box.boxBagData.itemList);
        }
    }


    private void OnBaseBagOpenEvent(SlotType slotType, InventorySlotData_SO bagData)
    {
        currentBoxData = bagData;
    }
    public bool CheckBagItemAmount(InventoryItem inventoryItem)
    {
        int count = 0;
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == inventoryItem.itemID)
                count += playerBag.itemList[i].itemAmount;
        }
        if (count >= inventoryItem.itemAmount)
            return true;

        return false;
    }


    private void RemoveItemAtplayerBag(string itemID, int itemAmount)
    {
        int count = itemAmount;
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == itemID)
            {
                if (playerBag.itemList[i].itemAmount > count)
                {
                    int amount = playerBag.itemList[i].itemAmount - count;
                    InventoryItem item = new InventoryItem { itemID = itemID, itemAmount = amount, itemUseTimes = playerBag.itemList[i].itemUseTimes };
                    playerBag.itemList[i] = item;
                }
                else
                {
                    InventoryItem item = new InventoryItem { itemID = "", itemAmount = 0, itemUseTimes = 0 };
                    playerBag.itemList[i] = item;
                }
                count -= Settings.itemMaxNumber;
                if (count <= 0)
                    break;
            }
        }
    }
    private void InitInventoryBag()
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            ItemDetail itemDetail = GetItemDetail(playerBag.itemList[i].itemID);
            if (itemDetail != null)
                playerBag.itemList[i] = new InventoryItem() { itemID = playerBag.itemList[i].itemID, itemAmount = playerBag.itemList[i].itemAmount, itemUseTimes = itemDetail.useTimes };
        }

        for (int i = 0; i < equipmentBag.itemList.Count; i++)
        {
            ItemDetail itemDetail = GetItemDetail(equipmentBag.itemList[i].itemID);
            if (itemDetail != null)
                equipmentBag.itemList[i] = new InventoryItem() { itemID = equipmentBag.itemList[i].itemID, itemAmount = equipmentBag.itemList[i].itemAmount, itemUseTimes = itemDetail.useTimes };
        }
    }
    public void ChangePlayerEquiment()
    {

        ItemDetail itemDetail = GetItemDetail(equipmentBag.itemList[0].itemID);
        EventHandler.CallChangePlayerEquiment(itemDetail);
        EventHandler.CallChangeCharacterEquiment(itemDetail, 0);
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.boxItemList = this.boxItemList;
        saveData.buildingIDnumberDict = this.buildingIDnumberDict;
        saveData.inventoryDict = this.playerBag.itemList;
        saveData.equipmentList = this.equipmentBag.itemList;

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        playerBag = Instantiate(playerBagTemp);
        equipmentBag = Instantiate(equipmentBagTemp);
        this.boxItemList = saveData.boxItemList;
        this.buildingIDnumberDict = saveData.buildingIDnumberDict;
        this.playerBag.itemList = saveData.inventoryDict;
        this.equipmentBag.itemList = saveData.equipmentList;
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Equipment, equipmentBag.itemList);
        ChangePlayerEquiment();

        EventHandler.CallChangeCharacterAnimatorEvent();
        EventHandler.CallSwitchAnimation(PlayerPartType.Hurt);
    }
    private void OnStartNewGameEvent(int index)
    {
        playerBag = Instantiate(playerBagTemp);
        equipmentBag = Instantiate(equipmentBagTemp);
        boxItemList.Clear();
        buildingIDnumberDict.Clear();
        InitInventoryBag();
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Player, playerBag.itemList);
        EventHandler.CallUpdateInventoryUIEvent(InventoryLocation.Equipment, equipmentBag.itemList);
        ChangePlayerEquiment();

        EventHandler.CallChangeCharacterAnimatorEvent();
        EventHandler.CallSwitchAnimation(PlayerPartType.Hurt);
    }

}
