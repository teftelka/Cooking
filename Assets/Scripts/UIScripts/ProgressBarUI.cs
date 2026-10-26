using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    // Progress bar of a table or cookware. Finds its IHasProgress in the parents
    // and is visible only while the process is running (between 0 and 1).
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Image progressBarFill;

        private IHasProgress source;

        private void Awake()
        {
            source = GetComponentInParent<IHasProgress>();
            if (source == null)
            {
                Debug.LogError($"{nameof(ProgressBarUI)} must be placed under a table or cookware with progress", this);
                return;
            }
            source.OnProgressChanged += HandleProgressChanged;
        }

        private void Start()
        {
            SetProgress(0f);
        }

        private void OnDestroy()
        {
            if (source != null)
                source.OnProgressChanged -= HandleProgressChanged;
        }

        private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProgress(e.progress);
        }

        private void SetProgress(float progress)
        {
            progressBarFill.fillAmount = Mathf.Clamp01(progress);

            // Decided by the new value (the old code looked at the previous fill and blinked for a frame)
            bool visible = progress > 0f && progress < 1f;
            if (gameObject.activeSelf != visible)
                gameObject.SetActive(visible);
        }
    }
}
