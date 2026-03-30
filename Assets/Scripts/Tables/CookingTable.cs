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
                    PlayerTest.Instance.SetObject(TakeObject());
                    return;
                }
            }
            
            if (playerProduct is CookingTool && !_hasObject)
            {
                SetObjectOnTable(playerProduct);
                PlayerTest.Instance.ClearObject();
            }
        }

        private void StartCooking()
        {
            _state = CookingTableState.Cooking;
        }
    }
}