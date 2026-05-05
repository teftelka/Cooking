using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<ProductSO, int> _resources = new();

    public event EventHandler<OnResourceChangedEventArgs> OnResourceChanged;
    public class OnResourceChangedEventArgs : EventArgs
    {
        public ProductSO productSO;
        public int newAmount;
    }

    private void Awake()
    {
        Instance = this;
    }

    public int GetAmount(ProductSO product)
    {
        _resources.TryGetValue(product, out int amount);
        return amount;
    }

    public void Add(ProductSO product, int amount)
    {
        if (!_resources.ContainsKey(product))
            _resources[product] = 0;

        _resources[product] += amount;
        OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs 
            { productSO = product, newAmount = _resources[product] });
    }

    public bool TrySpend(ProductSO product, int amount)
    {
        if (GetAmount(product) < amount)
            return false;

        _resources[product] -= amount;
        OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs 
            { productSO = product, newAmount = _resources[product] });
        return true;
    }
}