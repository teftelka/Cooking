using System;
using Tables;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarCookingToolUI : MonoBehaviour
{
    [SerializeField] private Image progressBarFill;
    [SerializeField] private CookingTool cookingTool;
    
    private void Start()
    {
        SetVisibility(false);
        cookingTool.OnProgressChanged += HandleProgressChanged;
        cookingTool.OnEmptyTool += HandleEmptyTool;
    }

    private void HandleEmptyTool(object sender, EventArgs e)
    {
        SetVisibility(false);
    }

    private void HandleProgressChanged(object sender, CookingTool.OnProgressChangedEventArgs e)
    {
        SetVisibility(!(progressBarFill.fillAmount >= 1f));
        progressBarFill.fillAmount = e.someProgress;
    }
    
    
    private void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
