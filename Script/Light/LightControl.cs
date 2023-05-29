using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public LightPattenData_SO lightData;
    private Light2D currentLight;
    private LightDetail currentLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    public void ChangeLightShift(LightShift lightShift, float timeDifference)
    {
        currentLightDetails = lightData.GetLightDetail(lightShift);
        if (timeDifference < Settings.lightChangeDuration)
        {
            var colorOffst = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeDifference;
            currentLight.color += colorOffst;

            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChangeDuration - timeDifference);
        }
        if (Mathf.Abs(timeDifference) >= Settings.lightChangeDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }

}
