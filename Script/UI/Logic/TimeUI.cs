using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform timeImage;
    public TMP_Text timeText;
    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }
    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }
    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
        var target = new Vector3(0, 0, hour * 15 - 90);
        timeImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
