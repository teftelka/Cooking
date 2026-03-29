using DefaultNamespace;

namespace Tables
{
    public class CuttingTable: BaseTable, IClickable
    {
        private enum CuttingTableState
        {
            Idle,
            Cutting,
            Cut
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
            
            if (playerProduct is Product && !_hasObject)
            {
                SetObjectOnTable(PlayerTest.Instance.GetProduct());
                PlayerTest.Instance.ClearObject();
                _product.Cut();
            }

        }
    }
}