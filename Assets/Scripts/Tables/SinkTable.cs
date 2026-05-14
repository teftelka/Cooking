using System;
using DefaultNamespace;
using Interfaces;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Tables
{
    public class SinkTable: BaseTable, IClickable
    {
        private float washTime = 3;
        private int platesCount = 0;
        [SerializeField] private SinkTableState washingState;
        [SerializeField] private float _timer;
        [SerializeField] private GameObject indicatorImage;
        
        public EventHandler<OnWashingProgressChangedEventArgs> OnWashingProgressChanged;
        public class OnWashingProgressChangedEventArgs {
            public float washingProgress; 
        }
        
        public EventHandler OnWashingCompleted;
        
        private enum SinkTableState
        {
            Idle,
            Washing
        }
        
        private void Start()
        {
            washingState = SinkTableState.Idle;
            OrderManager.Instance.OnOrderComplete += HandleOrderCompleted;
        }

        private void HandleOrderCompleted(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            AddDirtyPlate();
        }

        private void Update()
        {
            if (washingState != SinkTableState.Washing) return;
            
            _timer -= Time.deltaTime;
            OnWashingProgressChanged?.Invoke(this, new OnWashingProgressChangedEventArgs { washingProgress = 1 - (_timer / washTime) });
            
            if (_timer > 0f) return;
            
            FinishWashing();
            
            if (platesCount != 0) 
            {
                _timer = washTime;
                return;
            }
            
            indicatorImage.SetActive(false);
            washingState = SinkTableState.Idle;
        }

        private void FinishWashing()
        {
            OnWashingCompleted?.Invoke(this, EventArgs.Empty);
            platesCount--;
        }

        public void OnClick()
        {
            if (washingState == SinkTableState.Washing) return;
            if (platesCount == 0) return;
            StartWashing();
        }

        private void StartWashing()
        {
            washingState = SinkTableState.Washing;
            _timer = washTime;
        }
        
        private void AddDirtyPlate()
        {
            indicatorImage.SetActive(true);
            platesCount++;
        }
    }
}