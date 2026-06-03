using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ProductExperienceManager: MonoBehaviour
    {
        public static ProductExperienceManager Instance { get; private set; }

        [SerializeField] private int baseExperienceToLevelUp = 100;
        [SerializeField] private int extraExperiencePerLevel = 50;
        
        private readonly Dictionary<ProductSO, int> experienceDict = new();
        private readonly Dictionary<ProductSO, int> levelDict = new();
        
        public event EventHandler<OnExperienceChangedEventArgs> OnExperienceChanged;
        public event EventHandler<OnLevelChangedEventArgs> OnLevelChanged;
        public class OnExperienceChangedEventArgs : EventArgs
        {
            public ProductSO productSO;
            public int exp;
        }
        
        public class OnLevelChangedEventArgs : EventArgs
        {
            public ProductSO productSO;
            public int level;
        }

        private void Awake()
        {
            Instance = this;
        }
        
        public int GetExperience(ProductSO productSO)
        {
            experienceDict.TryGetValue(productSO, out int exp);
            return exp;
        }
        
        public int GetLevel(ProductSO productSO)
        {
            levelDict.TryGetValue(productSO, out int level);
            return level;
        }

        public int GetExperienceToNextLevel(ProductSO productSO)
        {
            int level = GetLevel(productSO);
            return baseExperienceToLevelUp + level * extraExperiencePerLevel;
        }

        /*public int GetAmount(ProductSO product)
        {
            experienceDict.TryGetValue(product, out int amount);
            return amount;
        }*/
        
        public void AddExperience(ProductSO productSO, int amount)
        {
            if (!productSO)
            {
                Debug.LogError("Cannot add experience: ProductSO is null.");
                return;
            }

            if (amount <= 0) return;

            if (!experienceDict.ContainsKey(productSO))
                experienceDict[productSO] = 0;

            experienceDict[productSO] += amount;

            TryLevelUp(productSO);

            OnExperienceChanged?.Invoke(this, new OnExperienceChangedEventArgs
            {
                productSO = productSO,
                exp = GetExperience(productSO)
            });
        }
        
        private void TryLevelUp(ProductSO productSO)
        {
            while (GetExperience(productSO) >= GetExperienceToNextLevel(productSO))
            {
                int requiredExp = GetExperienceToNextLevel(productSO);

                experienceDict[productSO] -= requiredExp;

                int newLevel = GetLevel(productSO) + 1;
                levelDict[productSO] = newLevel;

                OnLevelChanged?.Invoke(this, new OnLevelChangedEventArgs
                {
                    productSO = productSO,
                    level = newLevel
                });
            }
        }
    }
}