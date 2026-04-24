using System;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarTableUI : MonoBehaviour
{
    [SerializeField] private Image progressBarFill;
    [SerializeField] private CuttingTable cuttingTable;
    

    private void Start()
    {
        SetVisibility(false);
        cuttingTable.OnCuttingProgressChanged += HandleCuttingStateChanged;
    }

    private void HandleCuttingStateChanged(object sender, CuttingTable.OnCuttingProgressChangedEventArgs e)
    {
        SetVisibility(!(progressBarFill.fillAmount >= 0.99f));
        progressBarFill.fillAmount = e.cuttingProgress;
    }
    
    private void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
