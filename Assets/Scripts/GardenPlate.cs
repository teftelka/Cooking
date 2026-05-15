using System;
using Managers;
using UIScripts;
using UnityEngine;

public class GardenPlate: MonoBehaviour
{
    [SerializeField] private ProductSO productSO;
    [SerializeField] private GardenPlateUI gardenPlateUI;
    [SerializeField] private float timeToAddAmount = 10f;
    [SerializeField] private float _timer;
    private bool _isInitialized;

    
    public EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs {
        public float spawningProgress; 
    }
    
    private void Update()
    {
        if (!_isInitialized) return;

        _timer -= Time.deltaTime;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { spawningProgress = 1 - (_timer / timeToAddAmount) });
        
        if (_timer > 0f) return;
        ResourceManager.Instance.Add(productSO, 1);
        _timer = timeToAddAmount;
    }

    public void SetupGardenPlate(ProductSO nextProductSO)
    {
        productSO = nextProductSO;
        _timer = timeToAddAmount;
        SetVisuals();
        _isInitialized = true;
    }

    private void SetVisuals()
    {
        gardenPlateUI.SetProduct(productSO);
    }
}