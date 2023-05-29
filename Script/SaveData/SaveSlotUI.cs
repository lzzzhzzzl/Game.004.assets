using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private Button deleteButton;
    private DataSlot currentData;

    private int Index => transform.GetSiblingIndex();
    private void Awake()
    {
        currentButton = transform.GetChild(0).GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
        deleteButton = transform.GetChild(1).GetComponent<Button>();
        deleteButton.onClick.AddListener(DeletGameData);
    }
    private void OnEnable()
    {
        SetupSlotUI();
    }
    public void LoadGameData()
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            Debug.Log("新游戏");
            EventHandler.CallStartNewGameEvent(Index);
        }
    }
    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];

        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "-/-/-";
            dataScene.text = "新存档";
        }
    }
    private void DeletGameData()
    {
        currentData = null;
        SaveLoadManager.Instance.DeleteGameData(Index);
        dataTime.text = "-/-/-";
        dataScene.text = "新存档";
    }
}
