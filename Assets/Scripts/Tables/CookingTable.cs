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

        private void Start()
        {
            _state = CookingTableState.Idle;
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
                PlayerTest.Instance.HandleObjectTake(GiveObject());
            }
        }

        private void HandleCollision(BaseObject objectInHand)
        {
            if (_objectOnTable.CanAccept(objectInHand))
            {
                _objectOnTable.Accept(objectInHand);
                PlayerTest.Instance.HandleObjectGive();
            }
        }

        private void TakeObject(BaseObject objectInHand)
        {
            if (objectInHand is CookingTool cookingTool)
            {
                SetObjectOnTable(cookingTool);
                PlayerTest.Instance.HandleObjectGive();
            }
        }

        private void StartCooking()
        {
            _state = CookingTableState.Cooking;
            EndCooking();
        }

        private void EndCooking()
        {
            _state = CookingTableState.Idle;
        }
    }
}