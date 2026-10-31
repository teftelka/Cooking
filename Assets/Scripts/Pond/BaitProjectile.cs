using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pond
{
    // The thrown bait. The pond is seen from above, so the arc is shown by size:
    // the bait grows towards the middle of the flight and shrinks back as it lands.
    public class BaitProjectile : MonoBehaviour
    {
        public static BaitProjectile Spawn(Sprite sprite, Vector2 from, Vector2 to, float speed,
                                           float scale, float arcScale, float descentSlowdown,
                                           int sortingOrder, Scene scene, Action<Vector2> onLanded)
        {
            var go = new GameObject("Bait");
            if (scene.IsValid()) SceneManager.MoveGameObjectToScene(go, scene);
            go.transform.position = from;

            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;

            var bait = go.AddComponent<BaitProjectile>();
            bait.StartCoroutine(bait.Fly(from, to, speed, scale, arcScale, descentSlowdown, onLanded));
            return bait;
        }

        private IEnumerator Fly(Vector2 from, Vector2 to, float speed, float scale, float arcScale,
                                float descentSlowdown, Action<Vector2> onLanded)
        {
            // The rise takes half of the normal flight time, the fall is descentSlowdown times longer
            float normalDuration = Mathf.Max(Vector2.Distance(from, to) / Mathf.Max(speed, 0.01f), 0.15f);
            float riseTime = normalDuration * 0.5f;
            float fallTime = riseTime * Mathf.Max(descentSlowdown, 0.1f);
            float duration = riseTime + fallTime;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Flight progress: 0..0.5 while rising, 0.5..1 while falling
                float t = elapsed < riseTime
                    ? 0.5f * (elapsed / riseTime)
                    : 0.5f + 0.5f * Mathf.Clamp01((elapsed - riseTime) / fallTime);

                transform.position = Vector2.Lerp(from, to, t);
                float size = scale * Mathf.Lerp(1f, arcScale, Mathf.Sin(t * Mathf.PI));
                transform.localScale = new Vector3(size, size, 1f);
                yield return null;
            }

            transform.position = to;
            transform.localScale = new Vector3(scale, scale, 1f);
            onLanded?.Invoke(to);

            // Stays on the water for a moment so the player sees where it landed
            Destroy(gameObject, 0.6f);
        }
    }
}
