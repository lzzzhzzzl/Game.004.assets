using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFixed : MonoBehaviour, IBox
{
    public InventorySlotData_SO boxBagTemplate;
    public InventorySlotData_SO boxBagData;
    [Header("参数")]
    public List<string> itemCanChoose = new List<string>();
    [Range(0, 1)]
    public float range;
    private bool canOpen = false;
    private bool isOpen;

    public int index;
    private Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    public int GetIndex()
    {
        return index;
    }
    private void OnEnable()
    {
        if (boxBagData == null)
        {
            int num = 0;
            boxBagData = Instantiate(boxBagTemplate);
            for (int i = 0; i < boxBagData.itemList.Count * range; i++)
            {
                num = Random.Range(0, itemCanChoose.Count);
                ItemDetail itemDetail = InventoryManager.Instance.GetItemDetail(itemCanChoose[num]);
                if (itemDetail != null)
                    boxBagData.itemList[i] = new InventoryItem() { itemID = itemCanChoose[num], itemAmount = 1, itemUseTimes = itemDetail.useTimes };
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
        }
    }

    private void Update()
    {
        if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
        {
            EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
            anim.SetBool("isOpen", true);
            isOpen = true;
        }

        if (!canOpen && isOpen)
        {
            EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
            anim.SetBool("isOpen", false);
            isOpen = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
            anim.SetBool("isOpen", false);
            isOpen = false;
        }
    }

    public void InitBox(int boxIndex)
    {
        index = boxIndex;
        var key = this.name + index;
        if (InventoryManager.Instance.GetBoxFixedDataList(index) != null)
        {
            boxBagData.itemList = InventoryManager.Instance.GetBoxFixedDataList(index);
        }
        else  //新建箱子
        {
            InventoryManager.Instance.AddBoxDataList(this);
        }
    }
}
