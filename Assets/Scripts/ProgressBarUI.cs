using System;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image progressBarFill;
    [SerializeField] private CookingTable _cookingTable;
    private CookingTool _currentCookingTool;
    

    private void Start()
    {
        SetVisibility(false);
        _cookingTable.OnCookingToolChanged += HandleCookingToolChanged;
    }

    private void HandleCookingToolChanged(object sender, CookingTable.OnCookingToolChangeEventArgs e)
    {
        if (e.isCookingToolOnTable)
        {
            _currentCookingTool = e.cookingTool;
            _currentCookingTool.OnProgressChanged += HandleProgressChanged;
        }
        else
        {
            _currentCookingTool.OnProgressChanged -= HandleProgressChanged;
            _currentCookingTool = null;
            SetVisibility(false);
        }
    }

    private void HandleProgressChanged(object sender, CookingTool.OnProgressChangedEventArgs e)
    {
        SetVisibility(true);
        progressBarFill.fillAmount = e.someProgress;
    }
    
    
    private void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
