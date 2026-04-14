using System;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class CookingTable: BaseTable, IClickable
    {
        private enum CookingTableState
        {
            Idle,
            Cooking,
            Burning
        }

        [SerializeField] private CookingTableState _state;
        [SerializeField] private float cookingTime = 5f;
        [SerializeField] private float burningTime = 4f;
        private float _timer;

        private void Start()
        {
            _state = CookingTableState.Idle;
        }
        
        private void Update()
        {
            if (_state == CookingTableState.Idle) return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            var tool = (CookingTool)_objectOnTable;
            
            switch (_state)
            {
                case CookingTableState.Cooking:
                    tool.CookRecipe();
                    _state = CookingTableState.Burning;
                    _timer = burningTime;
                    Debug.Log("Food cooked");
                    break;
                case CookingTableState.Burning:
                    tool.BurnRecipe();
                    _state = CookingTableState.Idle;
                    Debug.Log("Food burned!");
                    break;
            }
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
            PlayerTest.Instance.HandleObjectTake(GiveObject());
            _state = CookingTableState.Idle;
        }

        private void HandleCollision(BaseObject objectInHand)
        {
            if (_objectOnTable.CanAccept(objectInHand))
            {
                _objectOnTable.Accept(objectInHand);
                PlayerTest.Instance.HandleObjectGive();
                StartCooking();
            }
        }

        private void TakeObject(BaseObject objectInHand)
        {
            if (objectInHand is CookingTool cookingTool)
            {
                SetObjectOnTable(cookingTool);
                PlayerTest.Instance.HandleObjectGive();
                if (cookingTool.CanCook())
                {
                    StartCooking();
                }
            }
        }

        private void StartCooking()
        {
            _timer = cookingTime;
            _state = CookingTableState.Cooking;
        }

        private void EndCooking()
        {
            _state = CookingTableState.Idle;
        }
    }
}