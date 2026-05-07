using System;
using System.Collections.Generic;
using UnityEngine;

public class ProductExperienceManager: MonoBehaviour
{
    public static ProductExperienceManager Instance { get; private set; }

    private Dictionary<ProductSO, int> experienceDict = new();

    public event EventHandler<OnExperienceChangedEventArgs> OnExperienceChanged;
    public class OnExperienceChangedEventArgs : EventArgs
    {
        public ProductSO productSO;
        public int exp;
    }

    private void Awake()
    {
        Instance = this;
    }

    public int GetAmount(ProductSO product)
    {
        experienceDict.TryGetValue(product, out int amount);
        return amount;
    }

    public void AddExperience(ProductSO productSO, int amount)
    {
        if (!experienceDict.ContainsKey(productSO))
            experienceDict[productSO] = 0;

        experienceDict[productSO] += amount;
        
        OnExperienceChanged?.Invoke(this, new OnExperienceChangedEventArgs 
            { productSO = productSO, exp = experienceDict[productSO] });
    }
}