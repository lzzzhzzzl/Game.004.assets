using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTable : MonoBehaviour
{
    public Transform uiIcon;
    private bool canDo;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canDo = true;
            uiIcon.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canDo = false;
            uiIcon.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (canDo && Input.GetMouseButtonDown(1))
        {
            EventHandler.CallIslandMapClickEvent(true);
        }
    }
}
