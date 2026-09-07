using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        private readonly Dictionary<ProductResourceKey, int> _resources = new();

        public event EventHandler<OnResourceChangedEventArgs> OnResourceChanged;
        public class OnResourceChangedEventArgs : EventArgs
        {
            public ProductSO productSO;
            public int range;
            public int newAmount;
        }

        private void Awake()
        {
            Instance = this;
        }

        public int GetAmount(ProductSO productSO, int range)
        {
            var key = new ProductResourceKey(productSO, range);
            _resources.TryGetValue(key, out int amount);
            return amount;
        }

        public void AddResource(ProductSO productSO, int range, int amount)
        {
            if (amount <= 0) return;

            var key = new ProductResourceKey(productSO, range);

            if (!_resources.ContainsKey(key))
                _resources[key] = 0;

            _resources[key] += amount;

            OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs
            {
                productSO = productSO,
                range = range,
                newAmount = _resources[key]
            });
        }

        public bool TrySpendResource(ProductSO productSO, int range, int amount)
        {
            if (productSO == null) return false;
            if (amount <= 0) return false;

            var key = new ProductResourceKey(productSO, range);

            if (GetAmount(productSO, range) < amount)
                return false;

            _resources[key] -= amount;

            OnResourceChanged?.Invoke(this, new OnResourceChangedEventArgs
            {
                productSO = productSO,
                range = range,
                newAmount = _resources[key]
            });

            return true;
        }
    }
}