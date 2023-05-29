using System;
using System.Collections.Generic;
using UnityEngine;
using Strategy.Map;

public class EventHandler
{
    public static event Action<SceneDetail, Vector3> TransitionInMapEvent;
    public static void CallTransitionInMapEvent(SceneDetail sceneToGo, Vector3 positionToBack)
    {
        TransitionInMapEvent?.Invoke(sceneToGo, positionToBack);
    }
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 position)
    {
        MoveToPosition?.Invoke(position);
    }
    public static event Action startGameEvent;
    public static void CallstartGameEvent()
    {
        startGameEvent?.Invoke();
    }



    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()
    {
        AfterSceneLoadEvent?.Invoke();
    }
    public static event Action BeforeSceneLoadEvent;
    public static void CallBeforeSceneLoadEvent()
    {
        BeforeSceneLoadEvent?.Invoke();
    }
    public static event Action LoadSceneDataEvent;
    public static void CallLoadSceneDataEvent()
    {
        LoadSceneDataEvent?.Invoke();
    }
    public static event Action AfterSceneDataLoadEvent;
    public static void CallAfterSceneDataLoadEvent()
    {
        AfterSceneDataLoadEvent?.Invoke();
    }


    public static event Action<string, Vector3, string> GenerateInventoryItemEvent;
    public static void CallGenerateInventoryItem(string inventoryID, Vector3 pos, string blockID)
    {
        GenerateInventoryItemEvent?.Invoke(inventoryID, pos, blockID);
    }
    public static event Action<string, Vector3> GenerateCropEvent;
    public static void CallGenerateCropEvent(string inventoryID, Vector3 pos)
    {
        GenerateCropEvent?.Invoke(inventoryID, pos);
    }
    public static event Action<string, Vector3, string> GenerateExteriorBuildingEvent;
    public static void CallGenerateExteriorBuildingEvent(string inventoryID, Vector3 pos, string blockID)
    {
        GenerateExteriorBuildingEvent?.Invoke(inventoryID, pos, blockID);
    }
    public static event Action<string, Vector3> GenerateInsideBuildingEvent;
    public static void CallGenerateInsideBuildingEvent(string inventoryID, Vector3 pos)
    {
        GenerateInsideBuildingEvent?.Invoke(inventoryID, pos);
    }
    public static event Action<BluePrintDetail, Vector3> GenerateFurnitureEvent;
    public static void CallGenerateFurnitureEvent(BluePrintDetail bluePrintDetail, Vector3 pos)
    {
        GenerateFurnitureEvent?.Invoke(bluePrintDetail, pos);
    }

    public static event Action<Sprite> StartSliderEvent;
    public static void CallStartSliderEvent(Sprite sprite)
    {
        StartSliderEvent?.Invoke(sprite);
    }
    public static event Action<float, string> SetSliderEvent;
    public static void CallSetSliderEvent(float loadable, string task)
    {
        SetSliderEvent?.Invoke(loadable, task);
    }
    public static event Action EndSliderEvent;
    public static void CallEndSliderEvent()
    {
        EndSliderEvent?.Invoke();
    }


    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUIEvent;
    public static void CallUpdateInventoryUIEvent(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUIEvent?.Invoke(location, list);
    }
    public static event Action<ItemDetail, int, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetail itemDetail, int index, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetail, index, isSelected);
    }
    public static event Action<string> HarvestAtPlayerPositionEvent;
    public static void CallHarvestAtPlayerPositionEvent(string itemID)
    {
        HarvestAtPlayerPositionEvent?.Invoke(itemID);
    }
    public static event Action ChangeCharacterAnimatorEvent;
    public static void CallChangeCharacterAnimatorEvent()
    {
        ChangeCharacterAnimatorEvent?.Invoke();
    }
    public static event Action<ItemDetail> ChangePlayerEquiment;
    public static void CallChangePlayerEquiment(ItemDetail itemDetail)
    {
        ChangePlayerEquiment?.Invoke(itemDetail);
    }
    public static event Action<PlayerPartName> RefreshPlayerAnimatorToNull;
    public static void CallRefreshPlayerAnimatorToNull(PlayerPartName partName)
    {
        RefreshPlayerAnimatorToNull?.Invoke(partName);
    }
    public static event Action<PlayerPartType> SwitchAnimation;
    public static void CallSwitchAnimation(PlayerPartType playerPartType)
    {
        SwitchAnimation?.Invoke(playerPartType);
    }
    public static event Action<ItemDetail, int> ChangeCharacterEquiment;
    public static void CallChangeCharacterEquiment(ItemDetail itemDetail, int index)
    {
        ChangeCharacterEquiment?.Invoke(itemDetail, index);
    }
    public static Action<Vector3, ItemDetail, int> MouseClickEvent;
    public static void CallMouseClickEvent(Vector3 mouseWorldPos, ItemDetail currentItem, int index)
    {
        MouseClickEvent?.Invoke(mouseWorldPos, currentItem, index);
    }
    public static event Action<Vector3, ItemDetail, int> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetail currentItem, int index)
    {
        ExecuteActionAfterAnimation?.Invoke(mouseWorldPos, currentItem, index);
    }
    public static event Action<string, Vector3> GenerateItemEvent;
    public static void CallGenerateItemEvent(string itemID, Vector3 pos)
    {
        GenerateItemEvent?.Invoke(itemID, pos);
    }
    public static event Action<BluePrintDetail> BluePrintSelectedEvent;
    public static void CallBluePrintSelectedEvent(BluePrintDetail bluePrintDetail)
    {
        BluePrintSelectedEvent?.Invoke(bluePrintDetail);
    }
    public static event Action<BluePrintDetail> BuildBluePrintEvent;
    public static void CallBuildBluePrintEvent(BluePrintDetail bluePrintDetail)
    {
        BuildBluePrintEvent?.Invoke(bluePrintDetail);
    }
    public static event Action<BluePrintDetail, Vector3Int> mouseClickBulePrintEvent;
    public static void CallmouseClickBulePrintEvent(BluePrintDetail bluePrintDetail, Vector3Int pos)
    {
        mouseClickBulePrintEvent?.Invoke(bluePrintDetail, pos);
    }

    public static event Action<SlotType, InventorySlotData_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType, InventorySlotData_SO bagData)
    {
        BaseBagOpenEvent?.Invoke(slotType, bagData);
    }

    public static event Action<SlotType, InventorySlotData_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventorySlotData_SO bagData)
    {
        BaseBagCloseEvent?.Invoke(slotType, bagData);
    }
    public static event Action ClearAllEnemyTargetTransfromEvent;
    public static void CallClearAllEnemyTargetTransfromEvent()
    {
        ClearAllEnemyTargetTransfromEvent?.Invoke();
    }
    public static event Action UpdatePlayerAnimatorEvent;
    public static void CallUpdatePlayerAnimatorEvent()
    {
        UpdatePlayerAnimatorEvent?.Invoke();
    }
    public static event Action<string, Vector3> GenerateParameterEvent;
    public static void CallGenerateParameterEvent(string parameterID, Vector3 position)
    {
        GenerateParameterEvent?.Invoke(parameterID, position);
    }
    public static event Action<BulletType, Vector3, Vector3, Transform> ButtleGenerateEvent;
    public static void CallButtleGenerateEvent(BulletType bulletType, Vector3 startPos, Vector3 targetPos, Transform startTransfrom)
    {
        ButtleGenerateEvent?.Invoke(bulletType, startPos, targetPos, startTransfrom);
    }
    public static event Action SetMapObstacleToCharacter;
    public static void CallSetMapObstacleToCharacter()
    {
        SetMapObstacleToCharacter?.Invoke();
    }
    public static event Action<bool> IslandMapClickEvent;
    public static void CallIslandMapClickEvent(bool isOpen)
    {
        IslandMapClickEvent?.Invoke(isOpen);
    }
    public static event Action<Vector3Int> IslandMapInputEvent;
    public static void CallIslandMapInputEvent(Vector3Int position)
    {
        IslandMapInputEvent?.Invoke(position);
    }
    public static event Action<SceneDetail> TransitionInIslandEvent;
    public static void CallTransitionInIslandEvent(SceneDetail sceneToGo)
    {
        TransitionInIslandEvent?.Invoke(sceneToGo);
    }
    public static event Action<Sprite, string> SetMapDetailUIEvent;
    public static void CallSetMapDetailUIEvent(Sprite mapSprite, string mapDescribe)
    {
        SetMapDetailUIEvent?.Invoke(mapSprite, mapDescribe);
    }
    public static event Action<bool> IsInCampSceneEvent;
    public static void CallIsInCampSceneEvent(bool isInCamp)
    {
        IsInCampSceneEvent?.Invoke(isInCamp);
    }

    public static event Action<bool> OpenBulePrintEvent;
    public static void CallOpenBulePrintEvent(bool isInCamp)
    {
        OpenBulePrintEvent?.Invoke(isInCamp);
    }
    public static event Action<float> HealthChangeEvent;
    public static void CallHealthChangeEvent(float health)
    {
        HealthChangeEvent?.Invoke(health);
    }

    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int gameMinute, int gameHour)
    {
        GameMinuteEvent?.Invoke(gameMinute, gameHour);
    }
    public static event Action<LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(LightShift lightShift, float gameHour)
    {
        LightShiftChangeEvent?.Invoke(lightShift, gameHour);
    }
    public static event Action<int, int, LightShift> GameDateEvent;
    public static void CallGameDataEvent(int gameHour, int gameDay, LightShift lightShift)
    {
        GameDateEvent?.Invoke(gameHour, gameDay, lightShift);
    }
    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }
    public static event Action EndGameEvent;
    public static void CallEndGameEvent()
    {
        EndGameEvent?.Invoke();
    }
    public static event Action<Vector3, ParticaleEffectType, float> ParticleGenerateEvent;
    public static void CallParticleGenerateEvent(Vector3 position, ParticaleEffectType particaleEffectType, float damage)
    {
        ParticleGenerateEvent?.Invoke(position, particaleEffectType, damage);
    }
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }
    public static event Action<SoundDetail> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetail soundDetail)
    {
        InitSoundEffect?.Invoke(soundDetail);
    }
    public static event Action GameOverEvent;
    public static void CallGameOverEvent()
    {
        GameOverEvent?.Invoke();
    }
    public static event Action ReLoadGameEvent;
    public static void CallReLoadGameEvent()
    {
        ReLoadGameEvent?.Invoke();
    }
    public static event Action CloseCurrentScene;
    public static void CallCloseCurrentScene()
    {
        CloseCurrentScene?.Invoke();
    }
    public static event Action UpdateLightControl;
    public static void CallUpdateLightControl()
    {
        UpdateLightControl?.Invoke();
    }
    public static event Action<InventoryLocation, int> ReduceItemDurability;
    public static void CallReduceItemDurability(InventoryLocation inventoryLocation, int index)
    {
        ReduceItemDurability?.Invoke(inventoryLocation, index);
    }

    public static event Action RemoveButtonClickEvent;
    public static void CallRemoveButtonClickEvent()
    {
        RemoveButtonClickEvent?.Invoke();
    }
    public static event Action<Transform, BluePrintDetail> RemoveFurnitureEvent;
    public static void CallRemoveFurnitureEvent(Transform transform, BluePrintDetail bluePrintDetail)
    {
        RemoveFurnitureEvent?.Invoke(transform, bluePrintDetail);
    }
}