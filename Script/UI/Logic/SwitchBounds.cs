using UnityEngine;
using Cinemachine;
using Strategy.Map;

public class SwitchBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneDataLoadEvent += OnAfterSceneDataLoadEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneDataLoadEvent -= OnAfterSceneDataLoadEvent;
    }
    private void OnAfterSceneDataLoadEvent()
    {
        GridMapManager.Instance.SetCurrentSceneBounds();
        PolygonCollider2D confinerShap = GameObject.FindWithTag("BoundConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShap;

        confiner.InvalidatePathCache();
    }
}
