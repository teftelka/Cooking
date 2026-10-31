using Interfaces;
using UnityEngine;

namespace Pond
{
    // The pond's controls: press inside the slingshot's grab circle, pull, release to throw.
    // Put it into the pond GameZone's "Active When Shown" and "Exit Blockers":
    // the zone can't be left while the string is pulled or a bait is in the air.
    public class PondInput : MonoBehaviour, IZoneExitBlocker
    {
        [SerializeField] private Slingshot slingshot;

        [Tooltip("Empty: the scene's PondManager")]
        [SerializeField] private PondManager pond;

        [Tooltip("Empty: the main camera")]
        [SerializeField] private Camera viewCamera;

        private GameInput subscribedTo;
        private bool isAiming;
        private int baitsInFlight;

        private PondManager Manager => pond != null ? pond : PondManager.Instance;
        private Camera ViewCamera => viewCamera != null ? viewCamera : Camera.main;

        public bool CanLeaveZone => !isAiming && baitsInFlight == 0;

        // GameInput lives in the Core scene and may wake up later, so try on enable and again on start
        private void OnEnable() => Subscribe();
        private void Start() => Subscribe();

        private void OnDisable()
        {
            Unsubscribe();
            if (isAiming) CancelAim();
        }

        private void Subscribe()
        {
            if (subscribedTo != null || GameInput.Instance == null) return;
            subscribedTo = GameInput.Instance;
            subscribedTo.OnPointerDown += HandlePointerDown;
            subscribedTo.OnPointerUp += HandlePointerUp;
        }

        private void Unsubscribe()
        {
            if (subscribedTo == null) return;
            subscribedTo.OnPointerDown -= HandlePointerDown;
            subscribedTo.OnPointerUp -= HandlePointerUp;
            subscribedTo = null;
        }

        private void Update()
        {
            if (isAiming && GameInput.Instance != null)
                slingshot.ShowPull(ToWorld(GameInput.Instance.GetMousePosition()));
        }

        private void HandlePointerDown(Vector2 screenPosition)
        {
            // Until fishing is opened the pond is only scenery: presses do nothing
            if (!UnlockManager.IsFeatureUnlocked(GameFeature.Fishing)) return;
            if (slingshot == null || !slingshot.isActiveAndEnabled || Manager == null) return;
            if (baitsInFlight > 0) return;   // one bait at a time
            if (!slingshot.IsInGrabZone(ToWorld(screenPosition))) return;

            if (Manager.Baits <= 0)
            {
                Debug.Log("Pond: no bait left, wait for it to come back");
                return;
            }

            isAiming = true;
        }

        private void HandlePointerUp(Vector2 screenPosition)
        {
            if (!isAiming) return;
            isAiming = false;

            if (!slingshot.TryGetTarget(ToWorld(screenPosition), out var target) || !Manager.TrySpendBait())
            {
                CancelAim();
                return;
            }

            slingshot.ResetString();
            baitsInFlight++;
            slingshot.Launch(target, landedAt =>
            {
                baitsInFlight--;
                Manager.ResolveLanding(landedAt);
            });
        }

        private void CancelAim()
        {
            isAiming = false;
            if (slingshot != null) slingshot.ResetString();
        }

        private Vector2 ToWorld(Vector2 screenPosition)
        {
            var cam = ViewCamera;
            return cam != null ? (Vector2)cam.ScreenToWorldPoint(screenPosition) : Vector2.zero;
        }
    }
}
