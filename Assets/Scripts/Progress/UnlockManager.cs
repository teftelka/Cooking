using System;
using System.Collections.Generic;
using UnityEngine;

// What the player has opened so far (fishing, later other mechanics). Lives in the Core scene:
// it is progress, so it later goes into the save.
// For testing, tick a feature in the Inspector, also during Play.
// From game conditions: UnlockManager.Instance.SetUnlocked(GameFeature.Fishing, true)
public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance { get; private set; }

    [Serializable]
    private class FeatureState
    {
        public GameFeature feature;
        public bool unlocked;
    }

    [Tooltip("Features missing from the list are closed")]
    [SerializeField] private List<FeatureState> features = new List<FeatureState>
    {
        new FeatureState { feature = GameFeature.Fishing, unlocked = false },
    };

    // The feature that was opened or closed
    public event Action<GameFeature> OnFeatureChanged;

    private void Awake()
    {
        Instance = this;
    }

    // Closed if there is no UnlockManager at all: a hidden mechanic must never show by mistake
    public static bool IsFeatureUnlocked(GameFeature feature) =>
        Instance != null && Instance.IsUnlocked(feature);

    public bool IsUnlocked(GameFeature feature)
    {
        foreach (var state in features)
        {
            if (state.feature == feature) return state.unlocked;
        }
        return false;
    }

    public void SetUnlocked(GameFeature feature, bool unlocked)
    {
        var state = features.Find(s => s.feature == feature);
        if (state == null)
        {
            state = new FeatureState { feature = feature };
            features.Add(state);
        }
        if (state.unlocked == unlocked) return;

        state.unlocked = unlocked;
        OnFeatureChanged?.Invoke(feature);
    }

#if UNITY_EDITOR
    // Ticking a feature in the Inspector during Play works like SetUnlocked
    private void OnValidate()
    {
        if (!Application.isPlaying || Instance != this) return;
        foreach (var state in features)
            OnFeatureChanged?.Invoke(state.feature);
    }
#endif
}
