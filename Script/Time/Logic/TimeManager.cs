using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour;
    /// <summary>
    /// 暂停时间
    /// </summary>
    private bool gameClockPause;
    /// <summary>
    /// 用于计时
    /// </summary>
    private int gameDay;
    private float tikTime;
    private float timeDifference;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);
    public string GUID => GetComponent<DataGUID>().guid;
    private void OnEnable()
    {
        EventHandler.BeforeSceneLoadEvent += OnBeforeSceneLoadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneLoadEvent -= OnBeforeSceneLoadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }
    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }
    private void OnStartNewGameEvent(int index)
    {
        gameMinute = 0;
        gameHour = 7;
        gameDay = 0;
    }

    private void OnBeforeSceneLoadEvent()
    {
        gameClockPause = true;
    }
    private void OnAfterSceneLoadEvent()
    {
        gameClockPause = false;
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
        EventHandler.CallLightShiftChangeEvent(GetCurrentLightShift(), timeDifference);
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }

    }

    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameSecond = 0;
            gameMinute++;
            if (gameMinute > Settings.minuteHold)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > Settings.hourHold)
                {
                    gameHour = 0;
                    gameDay++;
                }
                EventHandler.CallGameDataEvent(gameHour, gameDay, GetCurrentLightShift());
            }

            EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
            EventHandler.CallLightShiftChangeEvent(GetCurrentLightShift(), timeDifference);
        }
    }
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.mornimgTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.mornimgTime).TotalMinutes;
            return LightShift.Morning;
        }
        if (GameTime < Settings.mornimgTime || GameTime >= Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.nightTime).TotalMinutes;
            return LightShift.Night;
        }

        return LightShift.Morning;
    }
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict["gameMinute"] = this.gameMinute;
        saveData.timeDict["gameHour"] = this.gameHour;
        saveData.timeDict["gameDay"] = this.gameDay;
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.gameMinute = saveData.timeDict["gameMinute"];
        this.gameHour = saveData.timeDict["gameHour"];
        this.gameDay = saveData.timeDict["gameDay"];
    }
}
