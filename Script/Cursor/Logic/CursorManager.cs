using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Strategy.Map;
using UnityEngine.EventSystems;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    [Header("鼠标的可用图像")]
    public Sprite normal, tool, sword, item, throwItem, build;
    private RectTransform cursorCanvas;
    private RectTransform removeSelectRect;
    private Sprite currentSprite;
    private Image cursorImage;
    private Image buildImage;
    private Image removeSelectImage;
    private Camera mainCamera;// 主相机
    private Camera islandCamera;
    private Grid currentGrid;// 当前的Grid，Tilemap上挂载的组件，有地图的网格信息
    private Grid islandmapGrid;
    private Grid buildMapGrid;

    private Vector3 mouseWorldPos;// 鼠标在地图上的坐标
    private int currentIndex;
    private ItemDetail currentItem;// 当前选中物品的基本信息，主要用于背包，箱子，商店之类的
    private BluePrintDetail currentBluePrintDetail;
    private CursorType currentCursorType;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.RemoveButtonClickEvent += OnRemoveButtonClickEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BuildBluePrintEvent += OnBuildBluePrintEvent;
        EventHandler.IslandMapClickEvent += OnIslandMapClickEvent;
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.RemoveButtonClickEvent -= OnRemoveButtonClickEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BuildBluePrintEvent -= OnBuildBluePrintEvent;
        EventHandler.IslandMapClickEvent -= OnIslandMapClickEvent;
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneUnloadEvent;
    }


    private void Start()
    {
        islandmapGrid = GameObject.FindGameObjectWithTag("islandmapParent").transform.GetChild(0).GetComponent<Grid>();
        buildMapGrid = GameObject.FindGameObjectWithTag("GridBuildMapParent").transform.GetChild(0).GetComponent<Grid>();
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        islandCamera = GameObject.FindGameObjectWithTag("islandCamera").GetComponent<Camera>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        removeSelectRect = GameObject.FindGameObjectWithTag("InWorldCanvas").transform.GetChild(0).GetComponent<RectTransform>();
        removeSelectImage = GameObject.FindGameObjectWithTag("InWorldCanvas").transform.GetChild(0).GetComponent<Image>();
        buildImage.enabled = false;
        removeSelectImage.enabled = false;
        currentSprite = normal;
        SetCursorImage(normal);
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (!InteractWithUI() && cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;

        switch (currentCursorType)
        {
            case CursorType.Normal:
                SetCursorImage(normal);
                break;
            case CursorType.Islandmap:
                SetCursorImage(item);
                SetCursorPos();
                CheckIslandInput();
                break;
            case CursorType.Build:
                SetCursorImage(build);
                SetCursorPos();
                CheckBuildInput();
                break;
            case CursorType.Remove:
                SetCursorImage(currentSprite);
                SetCursorPos();
                CheckRemoveInput();
                break;
            case CursorType.UseItem:
                SetCursorImage(currentSprite);
                SetCursorPos();
                CheckPlayerInput();
                break;
        }
    }
    private void CheckRemoveInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);
            if (collider.GetComponent<Furniture>())
            {
                Vector3Int worldPosition = buildMapGrid.WorldToCell(collider.transform.position);
                BluePrintDetail bluePrintDetail = InventoryManager.Instance.GetBluePrintDetail(collider.GetComponent<Furniture>().furnitureID);
                EventHandler.CallRemoveFurnitureEvent(collider.transform, bluePrintDetail);
            }
            removeSelectImage.enabled = false;
            currentCursorType = CursorType.Normal;
            SetBuildBluePrintFalse();
        }

        if (Input.GetMouseButtonDown(1))
        {
            removeSelectImage.enabled = false;
            currentCursorType = CursorType.Normal;
            SetBuildBluePrintFalse();
        }
    }
    private void OnIslandMapClickEvent(bool isClick)
    {
        SetBuildBluePrintFalse();
        if (isClick)
            currentCursorType = CursorType.Islandmap;
        else
            currentCursorType = CursorType.Normal;
    }
    private void OnAfterSceneLoadEvent()
    {
        currentGrid = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Grid>();
    }

    private void OnBeforeSceneUnloadEvent()
    {
        currentCursorType = CursorType.Normal;
    }
    private void OnItemSelectedEvent(ItemDetail itemDetail, int index, bool isSelected)
    {
        if (!isSelected)
        {
            currentIndex = -1;
            currentItem = null;
            currentCursorType = CursorType.Normal;
            currentSprite = normal;
        }
        else
        {
            //WORKFLOW:物品使用实现功能：鼠标图片类型
            currentIndex = index;
            currentItem = itemDetail;
            currentSprite = itemDetail.itemType switch
            {
                ItemType.Consumable => item,
                ItemType.AxeTool => tool,
                ItemType.PickAxe => tool,
                ItemType.Sword => sword,
                ItemType.Clothes => item,
                ItemType.Material => item,
                ItemType.Throw => throwItem,
                _ => normal
            };
            currentCursorType = CursorType.UseItem;
        }
        SetBuildBluePrintFalse();
    }

    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 判断是否和UI互动
    /// </summary>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else
            return false;
    }
    private void SetCursorPos()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        if (currentCursorType == CursorType.Build)
        {
            if (!HaveFurnitrueInRadius(currentBluePrintDetail))
                SetCursorInValid();
            else
                SetCursorValid();
        }
        else if (currentCursorType == CursorType.Remove)
        {
            if (!HaveFurnitureInMousePosition())
            {
                removeSelectImage.enabled = false;
                SetCursorInValid();
            }
            else
            {
                removeSelectImage.enabled = true;
                SetCursorValid();
            }
        }
    }


    private void SetCursorValid()
    {
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }
    private void SetCursorInValid()
    {
        //修改鼠标UI的颜色，这里是红色，透明度减少60%，Color中的四个参数为，（红、黄、蓝、透明度）这里1为最大
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }


    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && !InteractWithUI())
        {
            EventHandler.CallMouseClickEvent(mouseWorldPos, currentItem, currentIndex);
        }
    }

    private void CheckBuildInput()
    {
        if (Input.GetMouseButtonDown(0) && !InteractWithUI())
        {
            Vector3Int worldPosition = buildMapGrid.WorldToCell(mouseWorldPos);
            EventHandler.CallmouseClickBulePrintEvent(currentBluePrintDetail, worldPosition);
            currentCursorType = CursorType.Normal;
            SetBuildBluePrintFalse();
        }

        if (Input.GetMouseButtonDown(1))
        {
            currentCursorType = CursorType.Normal;
            SetBuildBluePrintFalse();
        }
    }
    private void CheckIslandInput()
    {
        if (Input.GetMouseButtonDown(0) && !InteractWithUI())
        {
            Vector3 mouseIslandPosition = islandCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -islandCamera.transform.position.z));
            EventHandler.CallIslandMapInputEvent(islandmapGrid.WorldToCell(mouseIslandPosition));
        }
    }

    private bool HaveFurnitrueInRadius(BluePrintDetail bluePrintDetails)
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        Vector3Int worldPosition = buildMapGrid.WorldToCell(mousePosition);
        buildImage.transform.position = mainCamera.WorldToScreenPoint(worldPosition);
        return GridMapManager.Instance.CheckGridBuildMap(worldPosition, bluePrintDetails.size);
    }
    private bool HaveFurnitureInMousePosition()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        Collider2D collider = Physics2D.OverlapPoint(mousePosition);
        if (collider.GetComponent<Furniture>())
        {
            Vector3Int worldPosition = buildMapGrid.WorldToCell(collider.transform.position);
            removeSelectImage.transform.position = worldPosition;
            BluePrintDetail bluePrintDetail = InventoryManager.Instance.GetBluePrintDetail(collider.GetComponent<Furniture>().furnitureID);
            Vector2 size = bluePrintDetail.furnitureSpriteInUI.bounds.max - bluePrintDetail.furnitureSpriteInUI.bounds.min;
            removeSelectRect.sizeDelta = size;
            return true;
        }
        else
            return false;
    }
    private void OnBuildBluePrintEvent(BluePrintDetail bluePrintDetail)
    {
        buildImage.enabled = true;
        buildImage.sprite = bluePrintDetail.furnitureSpriteInUI;
        buildImage.SetNativeSize();
        currentBluePrintDetail = bluePrintDetail;
        currentCursorType = CursorType.Build;
    }
    private void SetBuildBluePrintFalse()
    {
        EventHandler.CallOpenBulePrintEvent(false);
        buildImage.enabled = false;
    }

    private void OnRemoveButtonClickEvent()
    {
        currentCursorType = CursorType.Remove;
    }
}
