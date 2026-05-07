using Tables;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class ProgressBarTableUI : MonoBehaviour
    {
        [SerializeField] private Image progressBarFill;
        [SerializeField] private BaseTable table;

        private void Start()
        {
            SetVisibility(false);
            switch (table)
            {
                case CuttingTable cuttingTable:
                    cuttingTable.OnCuttingProgressChanged += HandleCuttingStateChanged;
                    break;
                case SinkTable sinkTable:
                    sinkTable.OnWashingProgressChanged += OnWashingProgressChanged;
                    break;
                case MergeTable mergeTable:
                    mergeTable.OnProgressChanged += OnMergingProgressChanged;
                    break;
                default:
                    Debug.LogError("Table type not implemented for ProgressBarTableUI");
                    break;
            }
        }

        private void OnMergingProgressChanged(object sender, MergeTable.OnProgressChangedEventArgs e)
        {
            ProgressChanged(e.progress);
        }

        private void OnWashingProgressChanged(object sender, SinkTable.OnWashingProgressChangedEventArgs e)
        {
            ProgressChanged(e.washingProgress);
        }

        private void HandleCuttingStateChanged(object sender, CuttingTable.OnCuttingProgressChangedEventArgs e)
        {
            ProgressChanged(e.cuttingProgress);
        }
        
        private void ProgressChanged(float progress)
        {
            SetVisibility(!(progressBarFill.fillAmount >= 0.99f));
            progressBarFill.fillAmount = progress;
        }
    
        private void SetVisibility(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
