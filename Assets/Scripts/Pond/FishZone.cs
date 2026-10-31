using UnityEngine;

namespace Pond
{
    // A place in the pond where fish can be caught. Its collider is its size:
    // a big zone is easy to hit, a small one is hard. Add FishZoneMover to make it move.
    // Make the collider a trigger.
    [RequireComponent(typeof(Collider2D))]
    public class FishZone : MonoBehaviour
    {
        [Tooltip("Shown in the console for now. Empty: the object's name")]
        [SerializeField] private string zoneName;

        private Collider2D zoneCollider;

        public string ZoneName => string.IsNullOrEmpty(zoneName) ? name : zoneName;

        // Used to prefer the smaller of two overlapping zones
        public float Area
        {
            get
            {
                var size = zoneCollider.bounds.size;
                return size.x * size.y;
            }
        }

        private void Awake()
        {
            zoneCollider = GetComponent<Collider2D>();
        }

        private void OnEnable() => PondManager.Register(this);
        private void OnDisable() => PondManager.Unregister(this);

        public bool Contains(Vector2 point) => zoneCollider.OverlapPoint(point);
    }
}
