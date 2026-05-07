using System;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class CuttingTable: BaseTable, IClickable
    {
        
        public EventHandler<OnCuttingProgressChangedEventArgs> OnCuttingProgressChanged;
        public class OnCuttingProgressChangedEventArgs {
            public float cuttingProgress; 
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
            OnCuttingProgressChanged?.Invoke(this, new OnCuttingProgressChangedEventArgs { cuttingProgress = 1 - (_timer / cuttingTime) });
            
            if (_timer > 0f) return;
            
            ((Product)_objectOnTable).ApplyAction(ProductAction.Cut);
            cuttingState = CuttingTableState.Cut;
        }

        public void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (playerProduct)
            {
                if (_hasObject)
                {
                    HandleCollision(playerProduct);
                    return;
                }
                TakeObject(playerProduct);
                return;
            }
            if (_hasObject)
            {
                HandleObjectGive();
            }
        }

        private void HandleObjectGive()
        {
            if (cuttingState == CuttingTableState.Cutting) return;
            PlayerTest.Instance.HandleObjectTake(GiveObject());
            cuttingState = CuttingTableState.Idle;
        }

        private void HandleCollision(BaseObject playerProduct)
        {
            if (playerProduct.CanAccept(_objectOnTable))
            {
                playerProduct.Accept(_objectOnTable);
                if (playerProduct is Plate && _objectOnTable is CookingTool) return;
                _objectOnTable = null;
                _hasObject = false;
            }
        }

        private void TakeObject(BaseObject obj)
        {
            if (obj is not Product product) return;
            if (!product.CanApplyAction(ProductAction.Cut)) return;

            SetObjectOnTable(obj);
            PlayerTest.Instance.HandleObjectGive();
        }
        
        public override void SetObjectOnTable(BaseObject _object)
        {
            _hasObject = true;
            _objectOnTable = _object;
            _object.transform.position = spawnPosition.transform.position;
            _object.RememberOrigin(this);

            if (_object is Product product && product.CanApplyAction(ProductAction.Cut)) 
            {
                _timer = cuttingTime;
                cuttingState = CuttingTableState.Cutting;
            }
        }
    }
}