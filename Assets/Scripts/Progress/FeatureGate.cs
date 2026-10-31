using System.Collections.Generic;
using UnityEngine;

// Hides a mechanic until it is opened: the listed objects exist only while the feature is unlocked.
// Put it on an object that is always active (not on one of the listed objects).
public class FeatureGate : MonoBehaviour
{
    [SerializeField] private GameFeature feature;

    [Tooltip("Turned on when the feature is unlocked, off while it is closed")]
    [SerializeField] private List<GameObject> shownWhenUnlocked = new List<GameObject>();

    private UnlockManager subscribedTo;

    // Applied as early as possible, so the hidden objects never flash on screen
    private void Awake() => Apply();

    // UnlockManager lives in the Core scene and may wake up later, so try on enable and again on start
    private void OnEnable() => Subscribe();

    private void Start()
    {
        Subscribe();
        Apply();
    }

    private void OnDisable()
    {
        if (subscribedTo == null) return;
        subscribedTo.OnFeatureChanged -= HandleFeatureChanged;
        subscribedTo = null;
    }

    private void Subscribe()
    {
        if (subscribedTo != null || UnlockManager.Instance == null) return;
        subscribedTo = UnlockManager.Instance;
        subscribedTo.OnFeatureChanged += HandleFeatureChanged;
    }

    private void HandleFeatureChanged(GameFeature changed)
    {
        if (changed == feature) Apply();
    }

    private void Apply()
    {
        bool unlocked = UnlockManager.IsFeatureUnlocked(feature);
        foreach (var target in shownWhenUnlocked)
        {
            if (target != null && target.activeSelf != unlocked)
                target.SetActive(unlocked);
        }
    }
}
