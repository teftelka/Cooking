using System;
using Interfaces;
using UnityEngine;

namespace Tables
{
    public class CuttingTable: BaseTable, IHasProgress
    {
        
        public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;

        // One args object reused every frame instead of a new one per event
        private readonly ProgressChangedEventArgs progressArgs = new ProgressChangedEventArgs();

        private void RaiseProgress(float progress)
        {
            progressArgs.progress = progress;
            OnProgressChanged?.Invoke(this, progressArgs);
        }
        
        [SerializeField] private float cuttingTime = 3f;
        [SerializeField] private float _timer;
        [SerializeField] private CuttingTableState cuttingState;
        
        private enum CuttingTableState
        {
            Idle,
            Cutting,
            Cut
        }
        
        private void Start()
        {
            cuttingState = CuttingTableState.Idle;
        }

        private void Update()
        {
            if (cuttingState != CuttingTableState.Cutting) return;
            
            _timer -= Time.deltaTime;
            RaiseProgress(1 - (_timer / cuttingTime));
            
            if (_timer > 0f) return;
            
            ((Product)_objectOnTable).ApplyAction(ProductAction.Cut);
            cuttingState = CuttingTableState.Cut;
        }

        // Only products that can be cut
        public override bool CanPlace(BaseObject obj) =>
            obj is Product product && product.CanApplyAction(ProductAction.Cut);

        // Nothing leaves the board while it is being cut (not even into a plate)
        public override bool CanTake() => cuttingState != CuttingTableState.Cutting;

        // A cut product on the board swaps with an uncut one from the hand, which starts cutting.
        // Not while cutting (CanTake), and only for products that can be cut (CanPlace)
        public override bool AllowSwap => true;

        // Right-click may bring back any product taken from here, e.g. an already chopped one
        public override bool CanReturn(BaseObject obj) => obj is Product;

        protected override void OnPlaced(BaseObject obj)
        {
            if (obj is Product product && product.CanApplyAction(ProductAction.Cut))
            {
                _timer = cuttingTime;
                cuttingState = CuttingTableState.Cutting;
            }
        }

        protected override void OnTaken(BaseObject obj)
        {
            cuttingState = CuttingTableState.Idle;
        }
    }
}
