using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// A black screen over everything, faded in and out when the player goes to another zone,
// so the switch looks like going to another place. Builds its own canvas: no scene setup needed.
// While the screen is (partly) dark it blocks clicks on the UI and, through GameInput's UI check,
// on the tables too.
public class ScreenFader : MonoBehaviour
{
    private const int SortingOrderAboveAllUI = 1000;

    private CanvasGroup canvasGroup;

    public static ScreenFader Create(Transform parent)
    {
        var root = new GameObject("ScreenFader");
        root.transform.SetParent(parent, false);

        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = SortingOrderAboveAllUI;
        root.AddComponent<GraphicRaycaster>();

        var image = new GameObject("Black").AddComponent<Image>();
        image.transform.SetParent(root.transform, false);
        image.color = Color.black;
        var rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var fader = root.AddComponent<ScreenFader>();
        fader.canvasGroup = root.AddComponent<CanvasGroup>();
        fader.SetAlpha(0f);
        return fader;
    }

    // To black (alpha 1) or back to the game (alpha 0). Unscaled time: works even on pause
    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        canvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    // Instantly dark (e.g. while the zones are loading at start) or instantly clear
    public void SetDark(bool dark)
    {
        SetAlpha(dark ? 1f : 0f);
    }

    private void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        // Clear screen: clicks pass through again
        canvasGroup.blocksRaycasts = alpha > 0f;
    }
}
