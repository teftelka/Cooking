using System;
using Interfaces;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Tables
{
    public class SinkTable: BaseTable, IHasProgress
    {
        private float washTime = 3;
        private int platesCount = 0;
        [SerializeField] private SinkTableState washingState;
        [SerializeField] private float _timer;
        [SerializeField] private GameObject indicatorImage;
        
        public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;

        // One args object reused every frame instead of a new one per event
        private readonly ProgressChangedEventArgs progressArgs = new ProgressChangedEventArgs();

        private void RaiseProgress(float progress)
        {
            progressArgs.progress = progress;
            OnProgressChanged?.Invoke(this, progressArgs);
        }
        
        public event EventHandler OnWashingCompleted;
        
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
            RaiseProgress(1 - (_timer / washTime));
            
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

        // Starts washing; the hand is not used
        public override void Interact(IHand hand)
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