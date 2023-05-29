using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPoint : MonoBehaviour
{
    public string parameterID;
    private void Start()
    {
        GenerateParameter(parameterID);
    }
    public void GenerateParameter(string parameterID)
    {
        EventHandler.CallGenerateParameterEvent(parameterID, transform.position);
        Destroy(gameObject);
    }
}
