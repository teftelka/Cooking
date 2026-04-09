using System;
using DefaultNamespace;

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

        private CookingTableState _state;

        private void Start()
        {
            _state = CookingTableState.Idle;
        }

        public void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (!playerProduct)
            {
                if (_hasObject)
                {
                    PlayerTest.Instance.HandleObjectTake(GiveObject());
                    return;
                }
            }
            
            if (playerProduct && !_hasObject)
            {
                TakeObject(playerProduct);
            }
        }

        private void TakeObject(BaseObject _object)
        {
            if (_object is CookingTool)
            {
                SetObjectOnTable(_object);
                PlayerTest.Instance.HandleObjectGive();
            }
        }

        private void StartCooking()
        {
            _state = CookingTableState.Cooking;
        }
    }
}