using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Pond
{
    // The pond's rules: how much bait there is (it comes back by itself over time, also while
    // the player is in another zone) and what a landed bait has hit.
    // For now a hit is only written to the console.
    public class PondManager : MonoBehaviour
    {
        public static PondManager Instance { get; private set; }

        [Tooltip("The water rectangle (a Collider2D). A bait landing outside it is a miss")]
        [SerializeField] private Collider2D waterArea;

        [Header("Bait")]
        [SerializeField, Min(1)] private int maxBaits = 5;
        [Tooltip("Seconds for one bait to come back")]
        [SerializeField, Min(0.1f)] private float secondsPerBait = 20f;

        [Tooltip("Optional: shows the bait count")]
        [SerializeField] private TextMeshProUGUI baitText;

        // Static: a zone may wake up before the manager
        private static readonly List<FishZone> zones = new List<FishZone>();

        private int baits;
        private float regenTimer;

        public event Action OnBaitsChanged;

        public int Baits => baits;
        public int MaxBaits => maxBaits;
        public Collider2D WaterArea => waterArea;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => zones.Clear();

        public static void Register(FishZone zone)
        {
            if (!zones.Contains(zone)) zones.Add(zone);
        }

        public static void Unregister(FishZone zone) => zones.Remove(zone);

        private void Awake()
        {
            Instance = this;
            baits = maxBaits;
        }

        private void Update()
        {
            // Until fishing is opened the bait just stays full: nothing is spent or restored
            if (!UnlockManager.IsFeatureUnlocked(GameFeature.Fishing))
            {
                baits = maxBaits;
                regenTimer = 0f;
                return;
            }

            if (baits < maxBaits)
            {
                regenTimer += Time.deltaTime;
                if (regenTimer >= secondsPerBait)
                {
                    regenTimer -= secondsPerBait;
                    baits++;
                    OnBaitsChanged?.Invoke();
                }
            }
            else
            {
                regenTimer = 0f;
            }

            RefreshText();
        }

        public bool TrySpendBait()
        {
            if (baits <= 0) return false;
            baits--;
            OnBaitsChanged?.Invoke();
            return true;
        }

        // Called when a bait lands
        public void ResolveLanding(Vector2 point)
        {
            // Moving zones are moved by their transform, but their colliders only follow at the next
            // physics step. Sync now, so the check uses where the zones really are at this moment
            Physics2D.SyncTransforms();

            // Shows the exact landing point in the Scene view for 2 seconds (debug)
            DrawCross(point, Color.red, 2f);

            if (waterArea != null && !waterArea.OverlapPoint(point))
            {
                Debug.Log("Pond: missed the water");
                return;
            }

            var zone = FindZoneAt(point);
            if (zone == null)
            {
                Debug.Log($"Pond: miss at {point}");
                return;
            }

            Debug.Log($"Pond: hit the fish zone '{zone.ZoneName}'");
        }

        private static void DrawCross(Vector2 point, Color color, float duration)
        {
            const float size = 0.2f;
            Debug.DrawLine(point + new Vector2(-size, -size), point + new Vector2(size, size), color, duration);
            Debug.DrawLine(point + new Vector2(-size, size), point + new Vector2(size, -size), color, duration);
        }

        // With overlapping zones the smallest one wins: it is the harder target
        private static FishZone FindZoneAt(Vector2 point)
        {
            FishZone best = null;
            foreach (var zone in zones)
            {
                if (zone == null || !zone.isActiveAndEnabled || !zone.Contains(point)) continue;
                if (best == null || zone.Area < best.Area) best = zone;
            }
            return best;
        }

        private void RefreshText()
        {
            if (baitText == null) return;

            string text = $"Наживка: {baits}/{maxBaits}";
            if (baits < maxBaits)
                text += $"  (+1 через {Mathf.CeilToInt(secondsPerBait - regenTimer)} с)";
            baitText.text = text;
        }
    }
}
