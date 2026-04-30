using System;
using UnityEngine;

namespace UIScripts
{
    public class ScoreManager: MonoBehaviour
    {
        public static ScoreManager Instance { get; set; }
        
        [SerializeField] private int currentMoney;
        [SerializeField] private int currentExperience;

        public EventHandler OnExperienceUpdated;
        public EventHandler OnMoneyUpdated;
        
        
        private void Awake()
        {
            Instance = this;
        }
        
        private void Start()
        {
            OrderManager.Instance.OnOrderComplete += HandleOrderComplete;
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
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