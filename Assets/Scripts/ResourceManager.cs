using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<GameObject, int> _resources = new();

    public event EventHandler<OnResourceChangedEventArgs> OnResourceChanged;
    public class OnResourceChangedEventArgs : EventArgs
    {
        public GameObject productPrefab;
        public int newAmount;
    }

    private void Awake()
    {
        Instance = this;
    }

    public int GetAmount(GameObject productPrefab)
    {
        _resources.TryGetValue(productPrefab, out int amount);
        return amount;
    }

    public void Add(GameObject productPrefab, int amount)
    {
        if (!_resources.ContainsKey(productPrefab))
            _resources[productPrefab] = 0;

        _resources[productPrefab] += amount;
        OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs 
            { productPrefab = productPrefab, newAmount = _resources[productPrefab] });
    }

    public bool TrySpend(GameObject productPrefab, int amount)
    {
        if (GetAmount(productPrefab) < amount)
            return false;

        _resources[productPrefab] -= amount;
        OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs 
            { productPrefab = productPrefab, newAmount = _resources[productPrefab] });
        return true;
    }
}