using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShowCharacterEquiment : MonoBehaviour
{
    public Image[] equimentSprites;

    private void OnEnable()
    {
        EventHandler.ChangeCharacterEquiment += OnChangeCharacterEquiment;
    }
    private void OnDisable()
    {
        EventHandler.ChangeCharacterEquiment -= OnChangeCharacterEquiment;
    }
    private void OnChangeCharacterEquiment(ItemDetail itemDetail, int index)
    {
        if (itemDetail != null)
        {
            equimentSprites[index].enabled = true;
            equimentSprites[index].sprite = itemDetail.itemOnWorldSprite;
        }
        else
        {
            equimentSprites[index].enabled = false;
        }
    }
}
