using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instanceportal : MonoBehaviour
{
    public InstanceportalData_SO instanceportalData_SO;
    private float speed;
    private float timer;
    private Stack<string> parameterList = new Stack<string>();
    private string currentParameterID;

    private void Update()
    {
        if (parameterList.Count != 0)
        {
            timer += Time.deltaTime;
            if (timer > speed)
            {
                timer = 0;
                currentParameterID = parameterList.Pop();
                EventHandler.CallGenerateParameterEvent(currentParameterID, transform.position);
            }
        }
    }
    public void CheckGameDate(int gameHour, int gameDay)
    {
        InstanceDetail currentInstanceDetail = instanceportalData_SO.GetInstanceDetail(gameHour, gameDay % 8);
        if (currentInstanceDetail != null)
        {
            speed = currentInstanceDetail.speed;
            for (int i = 0; i < currentInstanceDetail.amount; i++)
            {
                parameterList.Push(currentInstanceDetail.parameterID);
            }
        }
    }

}
