using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "LightPattenData_SO", menuName = "Light/LightPattenData_SO", order = 0)]
public class LightPattenData_SO : ScriptableObject
{
    public List<LightDetail> lightDetails;

    public LightDetail GetLightDetail(LightShift lightShift)
    {
        return lightDetails.Find(i => i.lightShift == lightShift);
    }
}