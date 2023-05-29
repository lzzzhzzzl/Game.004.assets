using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InstanceportalData_SO", menuName = "Instance/InstanceDetailData_SO", order = 0)]
public class InstanceportalData_SO : ScriptableObject
{
    public List<InstanceDetail> instanceDetailList;
    public InstanceDetail GetInstanceDetail(int gameHour, int gameDay)
    {
        return instanceDetailList.Find(i => i.gameHour == gameHour && i.gameDay == gameDay);
    }
}