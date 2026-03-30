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
            
            if (playerProduct is Product product && !_hasObject)
            {
                if (product.GetProductState() == ProductState.Raw)
                {
                    SetObjectOnTable(playerProduct);
                    PlayerTest.Instance.ClearObject();
                    product.Cut();
                }
            }
        }
    }
}