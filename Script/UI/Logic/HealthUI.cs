using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text healthtext;
    public TMP_Text armorText;
    private void OnEnable()
    {
        EventHandler.HealthChangeEvent += OnHealthChangeEvent;
        EventHandler.ChangePlayerEquiment += OnChangePlayerEquiment;
    }
    private void OnDisable()
    {
        EventHandler.HealthChangeEvent -= OnHealthChangeEvent;
        EventHandler.ChangePlayerEquiment -= OnChangePlayerEquiment;
    }

    private void OnChangePlayerEquiment(ItemDetail itemDetail)
    {
        if (itemDetail == null)
            armorText.text = "0";
        else if (itemDetail.itemType == ItemType.Clothes)
            armorText.text = itemDetail.damage.ToString();
    }

    public void OnHealthChangeEvent(float value)
    {
        slider.DOValue(value, Settings.durationInHealth);
        healthtext.text = (value * Settings.playerMaxHealth).ToString() + "/" + Settings.playerMaxHealth.ToString();
    }
}
