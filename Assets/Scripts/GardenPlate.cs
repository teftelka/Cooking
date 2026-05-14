using System;
using UIScripts;
using UnityEngine;

public class GardenPlate: MonoBehaviour
{
    [SerializeField] private ProductSO productSO;
    [SerializeField] private GardenPlateUI gardenPlateUI;


    private void Awake()
    {
        
    }

    public void SetupGardenPlate(ProductSO nextProductSO)
    {
        productSO = nextProductSO;
        SetVisuals();
    }

    private void SetVisuals()
    {
        gardenPlateUI.SetProduct(productSO);
    }
}