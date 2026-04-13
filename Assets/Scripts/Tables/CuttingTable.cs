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
                    PlayerTest.Instance.HandleObjectTake(GiveObject());
                    return;
                }
            }
            
            if (playerProduct && !_hasObject)
            {
                TakeObject(playerProduct);
            }
        }

        private void TakeObject(BaseObject obj)
        {
            if (obj is not Product product) 
                return;
            
            if (!product.CanApplyAction(ProductAction.Cut))
                return;

            SetObjectOnTable(obj);
            PlayerTest.Instance.HandleObjectGive();

            product.ApplyAction(ProductAction.Cut);
        }
    }
}