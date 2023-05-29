using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MapDetailUI : Singleton<MapDetailUI>
{
    public Image image;
    public TMP_Text text;

    private void OnEnable()
    {
        EventHandler.SetMapDetailUIEvent += OnSetMapDetailUIEvent;
    }
    private void OnDisable()
    {
        EventHandler.SetMapDetailUIEvent -= OnSetMapDetailUIEvent;
    }
    private void OnSetMapDetailUIEvent(Sprite mapSprite, string mapDescribe)
    {
        image.sprite = mapSprite;
        text.text = mapDescribe;
    }
}
