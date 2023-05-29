using System.Linq;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Strategy.Map;

public class AnimationManager : Singleton<AnimationManager>
{
    [Header("身体和其他部分的动画组件")]
    public AnimatorTypeData_SO bodyAnimatorTypeData_SO;
    [Header("装备的动画组件")]
    public AnimatorTypeData_SO itemAnimatorTypeData_SO;
    private Dictionary<string, MapComponent[,]> mapComponentsDict = new Dictionary<string, MapComponent[,]>();
    private AnimationOverride playerAnimationOverride;
    protected override void Awake()
    {
        base.Awake();
        playerAnimationOverride = FindObjectOfType<AnimationOverride>();
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
        EventHandler.ChangePlayerEquiment += OnChangePlayerEquiment;
        EventHandler.ChangeCharacterAnimatorEvent += OnChangeCharacterAnimatorEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
        EventHandler.ChangePlayerEquiment -= OnChangePlayerEquiment;
        EventHandler.ChangeCharacterAnimatorEvent -= OnChangeCharacterAnimatorEvent;
    }
    private void OnAfterSceneDataLoadEvent()
    {
        MapComponent[,] sceneComposition = BlockManager.Instance.GetMapComponents();
        string sceneName = TransitionManager.Instance.GetCurrentSceneName();
        if (!mapComponentsDict.ContainsKey(sceneName))
            mapComponentsDict.Add(sceneName, sceneComposition);

    }
    private void OnChangeCharacterAnimatorEvent()
    {
        if (playerAnimationOverride != null)
        {
            CharacterDetail characterDetail = playerAnimationOverride.characterDetail;
            List<AnimatorType> animatorTypes = new List<AnimatorType>();
            animatorTypes.AddRange(bodyAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.hairID));
            animatorTypes.AddRange(bodyAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.bodyID));

            if (itemAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.clothesID) != null)
                animatorTypes.AddRange(itemAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.clothesID));
            else
                EventHandler.CallRefreshPlayerAnimatorToNull(PlayerPartName.Clothes);

            if (itemAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.toolID) != null)
                animatorTypes.AddRange(itemAnimatorTypeData_SO.GetAnimatorTypeList(characterDetail.toolID));
            else
                EventHandler.CallRefreshPlayerAnimatorToNull(PlayerPartName.Tool);

            playerAnimationOverride.animatorType = animatorTypes;
        }

    }
    private void OnChangePlayerEquiment(ItemDetail itemDetail)
    {
        if (itemDetail == null)
            playerAnimationOverride.characterDetail.clothesID = "";
        else if (itemDetail.itemType == ItemType.Clothes)
            playerAnimationOverride.characterDetail.clothesID = itemDetail.itemID;
    }
}
