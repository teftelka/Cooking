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
            Cooked,
            Burnt
        }

        [SerializeField] private CookingTableState _state;
        [SerializeField] private float cookingTime = 5f;
        private float _timer;

        private void Start()
        {
            _state = CookingTableState.Idle;
        }
        
        private void Update()
        {
            if (_state != CookingTableState.Cooking) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                var currentTool = (CookingTool)_objectOnTable;
                currentTool.CookRecipe(); 
                _state = CookingTableState.Idle; //Переделать потом в горение
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
            _objectOnTable = null;
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
            //EndCooking();
        }

        private void EndCooking()
        {
            _state = CookingTableState.Idle;
        }
    }
}