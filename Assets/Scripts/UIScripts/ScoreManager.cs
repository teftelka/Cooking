using System;
using UnityEngine;

namespace UIScripts
{
    public class ScoreManager: MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        [SerializeField] private int currentMoney;
        [SerializeField] private int currentExperience;

        public event EventHandler OnExperienceUpdated;
        public event EventHandler OnMoneyUpdated;
        
        
        private void Awake()
        {
            Instance = this;
        }
        
        // Called by the kitchen when an order is served. The kitchen calls the score, not the other way
        // round: the score lives in the core scene and must not depend on any zone
        public void RewardForOrder()
        {
            ExperienceAmountUpdate(555);
            MoneyAmountUpdate(100);
        }

        private void ExperienceAmountUpdate(int exp)
        {
            currentExperience += exp;
            OnExperienceUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public bool TrySpendMoney(int amount)
        {
            if (currentMoney < amount) return false;
            
            MoneyAmountUpdate(-amount);
            return true;
        }
        
        private void MoneyAmountUpdate(int money)
        {
            currentMoney += money;
            OnMoneyUpdated?.Invoke(this, EventArgs.Empty);
        }


        public int GetExpAmount()
        {
            return currentExperience;
        }
        
        public int GetMoneyAmount()
        {
            return currentMoney;
        }
    }
}