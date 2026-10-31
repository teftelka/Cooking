using UnityEngine;

namespace Pond
{
    // Moves a fish zone: back and forth along a line, or around a circle.
    // Without this component the zone stands still. The path is drawn in the Scene view.
    public class FishZoneMover : MonoBehaviour
    {
        private enum MoveMode
        {
            BackAndForth,
            Circle
        }

        [SerializeField] private MoveMode mode = MoveMode.BackAndForth;

        [Tooltip("Back and forth: from the start position to start + this offset")]
        [SerializeField] private Vector2 offset = new Vector2(2f, 0f);

        [Tooltip("Circle: radius around the start position")]
        [SerializeField, Min(0.01f)] private float radius = 1f;

        [Tooltip("Units per second")]
        [SerializeField, Min(0f)] private float speed = 1f;

        private Vector3 start;
        private float angle;

        private void Start()
        {
            start = transform.position;
        }

        private void Update()
        {
            switch (mode)
            {
                case MoveMode.BackAndForth:
                    float length = offset.magnitude;
                    if (length < 0.0001f) return;
                    float t = Mathf.PingPong(Time.time * speed / length, 1f);
                    t = t * t * (3f - 2f * t);   // slows down at the ends
                    transform.position = start + (Vector3)(offset * t);
                    break;

                case MoveMode.Circle:
                    angle += speed / radius * Time.deltaTime;
                    transform.position = start + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = Application.isPlaying ? start : transform.position;
            Gizmos.color = Color.cyan;

            if (mode == MoveMode.BackAndForth)
            {
                Gizmos.DrawLine(origin, origin + (Vector3)offset);
                return;
            }

            const int segments = 32;
            for (int i = 0; i < segments; i++)
            {
                float a1 = i * Mathf.PI * 2f / segments;
                float a2 = (i + 1) * Mathf.PI * 2f / segments;
                Gizmos.DrawLine(origin + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1)) * radius,
                                origin + new Vector3(Mathf.Cos(a2), Mathf.Sin(a2)) * radius);
            }
        }
    }
}
