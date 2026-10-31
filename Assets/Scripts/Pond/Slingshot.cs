using System;
using UnityEngine;

namespace Pond
{
    // The slingshot in the middle of the water's bottom edge. Pressing inside the grab circle
    // and pulling stretches the string; releasing throws the bait the opposite way.
    // The harder the pull, the farther it flies. There is no landing marker: aiming is a skill.
    public class Slingshot : MonoBehaviour
    {
        [Tooltip("The water rectangle. The slingshot stands in the middle of its bottom edge")]
        [SerializeField] private Collider2D waterArea;

        [Header("Pulling")]
        [Tooltip("Pressing inside this radius around the slingshot grabs the string")]
        [SerializeField, Min(0.1f)] private float grabRadius = 1f;
        [Tooltip("How far the string stretches")]
        [SerializeField, Min(0.1f)] private float maxPull = 1.5f;
        [Tooltip("A shorter pull is cancelled: no bait is thrown")]
        [SerializeField, Min(0f)] private float minPull = 0.2f;

        [Header("Throw")]
        [Tooltip("Flight distance at full pull. 0 = the height of the water")]
        [SerializeField, Min(0f)] private float maxDistance = 0f;
        [Tooltip("Units per second")]
        [SerializeField, Min(0.1f)] private float flightSpeed = 8f;

        [Header("Look (placeholders)")]
        [SerializeField] private Sprite baitSprite;
        [SerializeField] private float baitScale = 0.4f;
        [Tooltip("How much bigger the bait gets in the middle of the flight")]
        [SerializeField] private float arcScale = 1.8f;
        [Tooltip("How much slower the fall is than the rise: 1 = the same, 1.5 = one and a half times slower")]
        [SerializeField, Min(0.1f)] private float descentSlowdown = 1.5f;
        [Tooltip("Optional: the visible grab circle, scaled to the grab radius")]
        [SerializeField] private Transform grabCircle;
        [Tooltip("Optional: a LineRenderer for the string. Created automatically if empty")]
        [SerializeField] private LineRenderer stringLine;
        [Tooltip("Half the distance between the slingshot's two prongs")]
        [SerializeField] private float prongHalfWidth = 0.5f;
        [SerializeField] private int sortingOrder = 20;

        public Vector2 Anchor => transform.position;

        private void Start()
        {
            if (waterArea != null)
            {
                var bounds = waterArea.bounds;
                transform.position = new Vector3(bounds.center.x, bounds.min.y, transform.position.z);
                if (maxDistance <= 0f) maxDistance = bounds.size.y;
            }
            if (maxDistance <= 0f) maxDistance = 5f;

            FitGrabCircle();

            CreateStringIfMissing();
            ResetString();
        }

        // Scales the visible circle so its diameter is exactly twice the grab radius,
        // whatever the size of its sprite (e.g. the 2D package circle is 2.56 units wide)
        private void FitGrabCircle()
        {
            if (grabCircle == null) return;

            float spriteDiameter = 1f;
            var spriteRenderer = grabCircle.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
                spriteDiameter = Mathf.Max(spriteRenderer.sprite.bounds.size.x, 0.0001f);

            float parentScale = grabCircle.parent != null ? Mathf.Max(grabCircle.parent.lossyScale.x, 0.0001f) : 1f;
            float scale = grabRadius * 2f / spriteDiameter / parentScale;
            grabCircle.localScale = new Vector3(scale, scale, 1f);
        }

        public bool IsInGrabZone(Vector2 worldPoint) =>
            (worldPoint - Anchor).sqrMagnitude <= grabRadius * grabRadius;

        // While pulling: the string follows the pointer, up to maxPull
        public void ShowPull(Vector2 pointerWorld)
        {
            var pull = Vector2.ClampMagnitude(pointerWorld - Anchor, maxPull);
            SetString(Anchor + pull);
        }

        public void ResetString() => SetString(Anchor);

        // Where the bait would land if released now. False if the pull is too short (cancelled)
        public bool TryGetTarget(Vector2 pointerWorld, out Vector2 target)
        {
            var pull = Vector2.ClampMagnitude(pointerWorld - Anchor, maxPull);
            target = Anchor;
            if (pull.magnitude < minPull) return false;

            float power = pull.magnitude / maxPull;
            target = Anchor - pull.normalized * (power * maxDistance);
            return true;
        }

        public void Launch(Vector2 target, Action<Vector2> onLanded)
        {
            BaitProjectile.Spawn(baitSprite, Anchor, target, flightSpeed, baitScale, arcScale,
                                 descentSlowdown, sortingOrder + 1, gameObject.scene, onLanded);
        }

        private void SetString(Vector2 middle)
        {
            if (stringLine == null) return;
            stringLine.SetPosition(0, Anchor + Vector2.left * prongHalfWidth);
            stringLine.SetPosition(1, middle);
            stringLine.SetPosition(2, Anchor + Vector2.right * prongHalfWidth);
        }

        private void CreateStringIfMissing()
        {
            if (stringLine == null)
            {
                var go = new GameObject("String");
                go.transform.SetParent(transform, false);
                stringLine = go.AddComponent<LineRenderer>();

                var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                if (shader != null) stringLine.material = new Material(shader);

                stringLine.startWidth = 0.06f;
                stringLine.endWidth = 0.06f;
                stringLine.startColor = new Color(0.35f, 0.2f, 0.1f);
                stringLine.endColor = new Color(0.35f, 0.2f, 0.1f);
            }

            stringLine.useWorldSpace = true;
            stringLine.positionCount = 3;
            stringLine.sortingOrder = sortingOrder;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            DrawCircle(transform.position, grabRadius);
            Gizmos.color = new Color(1f, 0.5f, 0f);
            DrawCircle(transform.position, maxPull);
        }

        private static void DrawCircle(Vector3 center, float radius)
        {
            const int segments = 32;
            for (int i = 0; i < segments; i++)
            {
                float a1 = i * Mathf.PI * 2f / segments;
                float a2 = (i + 1) * Mathf.PI * 2f / segments;
                Gizmos.DrawLine(center + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1)) * radius,
                                center + new Vector3(Mathf.Cos(a2), Mathf.Sin(a2)) * radius);
            }
        }
    }
}
