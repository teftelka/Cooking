using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    // Switches between the zones (kitchen, garden, pond) like between separate screens:
    // the screen fades to black, the camera and the UI switch, the screen fades back in.
    // Zones register themselves (GameZone), so they can live in other scenes.
    public class AreaManager : MonoBehaviour
    {
        public static AreaManager Instance { get; private set; }

        [SerializeField] private Camera mainCamera;

        [Header("Transition")]
        [Tooltip("Off: the zones switch instantly, without darkening the screen")]
        [SerializeField] private bool useFade = true;
        [SerializeField] private float fadeOutDuration = 0.2f;
        [SerializeField] private float fadeInDuration = 0.2f;

        // Static: a zone may register before this manager wakes up (another scene, other load order)
        private static readonly List<GameZone> zones = new List<GameZone>();
        private static GameAreaType currentArea = GameAreaType.Kitchen;

        private ScreenFader fader;
        private bool isSwitching;

        // Reset on entering Play mode, also when domain reload is turned off
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            zones.Clear();
            currentArea = GameAreaType.Kitchen;
        }

        public static void Register(GameZone zone)
        {
            if (!zones.Contains(zone)) zones.Add(zone);

            bool isCurrent = zone.ZoneType == currentArea;
            zone.SetShown(isCurrent);
            if (isCurrent && Instance != null)
                Instance.MoveCamera(zone.CameraPoint);
        }

        public static void Unregister(GameZone zone)
        {
            zones.Remove(zone);
        }

        private void Awake()
        {
            Instance = this;
            if (mainCamera == null)
                mainCamera = Camera.main;

            fader = ScreenFader.Create(transform);
        }

        private void Start()
        {
            var zone = GetZone(currentArea);
            if (zone != null)
                MoveCamera(zone.CameraPoint);
        }

        public GameAreaType GetCurrentArea() => currentArea;

        // At start, while the zone scenes are loading: the screen stays dark and zones can't be switched
        public void CoverScreen()
        {
            isSwitching = true;
            fader.SetDark(true);
        }

        public IEnumerator RevealScreen()
        {
            yield return fader.FadeTo(0f, fadeInDuration);
            isSwitching = false;
        }

        // False during a switch and while something in the current zone forbids leaving
        // (e.g. an object in the hand)
        public bool CanLeaveCurrentArea
        {
            get
            {
                if (isSwitching) return false;
                var zone = GetZone(currentArea);
                return zone == null || zone.CanLeave;
            }
        }

        public void ShowCooking() => ShowArea(GameAreaType.Kitchen);
        public void ShowGarden() => ShowArea(GameAreaType.Garden);
        public void ShowPond() => ShowArea(GameAreaType.Pond);

        public void ShowArea(GameAreaType targetArea)
        {
            if (targetArea == currentArea) return;
            if (!CanLeaveCurrentArea) return;

            var target = GetZone(targetArea);
            if (target == null)
            {
                Debug.LogError($"Zone {targetArea} is not in the game (no GameZone registered for it)");
                return;
            }

            if (useFade) StartCoroutine(SwitchWithFade(target));
            else SwitchTo(target);
        }

        private IEnumerator SwitchWithFade(GameZone target)
        {
            isSwitching = true;

            yield return fader.FadeTo(1f, fadeOutDuration);
            SwitchTo(target);
            yield return fader.FadeTo(0f, fadeInDuration);

            isSwitching = false;
        }

        private void SwitchTo(GameZone target)
        {
            var current = GetZone(currentArea);
            if (current != null) current.SetShown(false);

            currentArea = target.ZoneType;
            target.SetShown(true);
            MoveCamera(target.CameraPoint);
        }

        private static GameZone GetZone(GameAreaType type)
        {
            foreach (var zone in zones)
            {
                if (zone != null && zone.ZoneType == type) return zone;
            }
            return null;
        }

        private void MoveCamera(Transform target)
        {
            if (target == null || mainCamera == null) return;

            mainCamera.transform.position = target.position;
            mainCamera.transform.rotation = target.rotation;
        }
    }
}
