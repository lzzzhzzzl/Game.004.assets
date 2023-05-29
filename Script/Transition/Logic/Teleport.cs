using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("场景的名称")]
    public SceneDetail sceneToGo;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneDetail sceneDetail = TransitionManager.Instance.GetCurrentScene();
            sceneToGo.seed = sceneDetail.seed;
            EventHandler.CallTransitionInMapEvent(sceneToGo, other.transform.position + Vector3.down * 2);
        }
    }
}
