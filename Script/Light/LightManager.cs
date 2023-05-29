using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : Singleton<LightManager>
{
    private LightControl[] sceneLightList;
    private LightShift currentLightShift;

    private float timeDifference = Settings.lightChangeDuration;

    private void OnEnable()
    {
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
        EventHandler.UpdateLightControl += OnUpdateLightControl;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        EventHandler.UpdateLightControl += OnUpdateLightControl;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int index)
    {
        currentLightShift = LightShift.Morning;
    }
    public void OnUpdateLightControl()
    {
        sceneLightList = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLightList)
        {
            light.ChangeLightShift(currentLightShift, timeDifference);
        }
    }
    private void OnLightShiftChangeEvent(LightShift lightShift, float timeDifference)
    {
        if (sceneLightList == null)
            return;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLightList)
            {
                light.ChangeLightShift(currentLightShift, timeDifference);
            }
        }
    }

    private void OnAfterSceneDataLoadEvent()
    {
        OnUpdateLightControl();
    }

}
