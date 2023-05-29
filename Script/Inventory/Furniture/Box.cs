using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IBox
{
    public InventorySlotData_SO boxBagTemplate;
    public InventorySlotData_SO boxBagData;
    private bool canOpen = false;
    private bool isOpen;

    private int index;
    private Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (boxBagData == null)
        {
            boxBagData = Instantiate(boxBagTemplate);
        }

    }
    public int GetIndex()
    {
        return index;
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
        if (InventoryManager.Instance.GetBoxDataList(index) != null)
        {
            boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(index);
        }
        else  //新建箱子
        {
            InventoryManager.Instance.AddBoxDataList(this);
        }
    }
}
