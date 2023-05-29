using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour
{
    /// <summary>
    /// 动画器的列表
    /// </summary>
    private Animator[] animators;

    /// <summary>
    /// 玩家举起的物品的渲染器组件
    /// </summary>
    [Header("玩家举起的物品的图像渲染器")]
    public SpriteRenderer holdItem;
    [Header("无动画的覆盖器")]
    public AnimatorOverrideController noAnimator;

    /// <summary>
    /// 各部分的动画列表，用于执行不同动画
    /// </summary>
    [Header("各部分动画列表")]
    public List<AnimatorType> animatorType;

    /// <summary>
    /// 存储玩家身体不同部分的动画控制组件
    /// </summary>
    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();
    public CharacterDetail characterDetail;
    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.SwitchAnimation += OnSwitchAnimation;
        EventHandler.UpdatePlayerAnimatorEvent += OnUpdatePlayerAnimatorEvent;
        EventHandler.RefreshPlayerAnimatorToNull += OnCallRefreshPlayerAnimatorToNull;
        EventHandler.HarvestAtPlayerPositionEvent += OnHarvestAtPlayerPositionEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.SwitchAnimation -= OnSwitchAnimation;
        EventHandler.UpdatePlayerAnimatorEvent -= OnUpdatePlayerAnimatorEvent;
        EventHandler.RefreshPlayerAnimatorToNull -= OnCallRefreshPlayerAnimatorToNull;
        EventHandler.HarvestAtPlayerPositionEvent -= OnHarvestAtPlayerPositionEvent;
    }


    private void OnItemSelectedEvent(ItemDetail itemDetail, int index, bool isSelected)
    {
        //WORKFLOW:物品使用实现功能：动画
        PlayerPartType currentType = itemDetail.itemType switch
        {
            ItemType.Consumable => PlayerPartType.Carry,
            ItemType.Material => PlayerPartType.Carry,
            ItemType.AxeTool => PlayerPartType.Hoe,
            ItemType.PickAxe => PlayerPartType.Break,
            ItemType.Sword => PlayerPartType.Sword,
            _ => PlayerPartType.Hurt
        };
        if (isSelected == false)
        {
            currentType = PlayerPartType.Hurt;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PlayerPartType.Carry)
            {
                holdItem.enabled = true;
                holdItem.sprite = itemDetail.itemOnWorldSprite;
            }
            else
            {
                holdItem.enabled = false;
            }
        }
        //WORKFLOW: 添加额外动作类型时，需要在这里添加
        if (itemDetail.itemType == ItemType.PickAxe || itemDetail.itemType == ItemType.Sword || itemDetail.itemType == ItemType.AxeTool)
            characterDetail.toolID = itemDetail.itemID;
        else
            characterDetail.toolID = "";

        EventHandler.CallChangeCharacterAnimatorEvent();
        OnSwitchAnimation(currentType);
    }
    public void OnSwitchAnimation(PlayerPartType partType)
    {
        foreach (var item in animatorType)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
    private void OnUpdatePlayerAnimatorEvent()
    {
        ItemDetail itemDetail = InventoryManager.Instance.GetItemDetail(characterDetail.toolID);
        if (itemDetail != null)
        {
            if (itemDetail.itemType == ItemType.Sword)
                OnSwitchAnimation(PlayerPartType.Sword);
            else if (itemDetail.itemType == ItemType.AxeTool)
                OnSwitchAnimation(PlayerPartType.Hoe);
            else if (itemDetail.itemType == ItemType.PickAxe)
                OnSwitchAnimation(PlayerPartType.Break);
            else if (itemDetail.itemType == ItemType.Material)
                OnSwitchAnimation(PlayerPartType.Carry);
        }
        else
            OnSwitchAnimation(PlayerPartType.Hurt);
    }

    private void OnCallRefreshPlayerAnimatorToNull(PlayerPartName partName)
    {
        animatorNameDict[partName.ToString()].runtimeAnimatorController = noAnimator;
    }

    /// <summary>
    /// 显示头顶物品的信息
    /// </summary>
    private void OnHarvestAtPlayerPositionEvent(string itemID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetail(itemID).itemOnWorldSprite;
        if (holdItem.enabled == false)
            StartCoroutine(ShowItem(itemSprite));
    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.enabled = true;
        holdItem.sprite = itemSprite;
        yield return new WaitForSeconds(0.25f);
        holdItem.enabled = false;
    }

}
