using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherUICheck : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EventHandler.CallIslandMapClickEvent(false);
        }
    }
}
