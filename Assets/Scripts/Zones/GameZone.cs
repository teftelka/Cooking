using System.Collections.Generic;
using Interfaces;
using Managers;
using UnityEngine;

// One screen of the game: kitchen, garden or pond. Lives in its zone's scene and registers itself,
// so the zone switcher (AreaManager) never needs inspector links to zone objects.
// A hidden zone keeps running (the kitchen keeps cooking, the garden keeps growing);
// only its UI and the components listed below are switched off.
public class GameZone : MonoBehaviour
{
    [SerializeField] private GameAreaType zoneType;
    [SerializeField] private Transform cameraPoint;

    [Tooltip("UI shown only while this zone is on screen")]
    [SerializeField] private GameObject zoneUI;

    [Tooltip("Components that work only while this zone is on screen, e.g. the zone's input")]
    [SerializeField] private List<Behaviour> activeWhenShown = new List<Behaviour>();

    [Tooltip("Things that can forbid leaving this zone (IZoneExitBlocker), e.g. the player's hand")]
    [SerializeField] private List<MonoBehaviour> exitBlockers = new List<MonoBehaviour>();

    public GameAreaType ZoneType => zoneType;
    public Transform CameraPoint => cameraPoint;

    public bool CanLeave
    {
        get
        {
            foreach (var behaviour in exitBlockers)
            {
                if (behaviour is IZoneExitBlocker blocker && !blocker.CanLeaveZone) return false;
            }
            return true;
        }
    }

    private void OnEnable() => AreaManager.Register(this);
    private void OnDisable() => AreaManager.Unregister(this);

    public void SetShown(bool shown)
    {
        if (zoneUI != null) zoneUI.SetActive(shown);

        foreach (var behaviour in activeWhenShown)
        {
            if (behaviour != null) behaviour.enabled = shown;
        }
    }
}
