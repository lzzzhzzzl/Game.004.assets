using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUpandDown : MonoBehaviour
{
    public bool isUp;
    public int scale;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (isUp)
                other.transform.position += Vector3.up * scale;
            else
                other.transform.position += Vector3.down * scale;
        }
    }
}
