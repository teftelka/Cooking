using System;
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

        private void HandleCollision(BaseObject playerProduct)
        {
            if (playerProduct.CanAccept(_objectOnTable))
            {
                playerProduct.Accept(_objectOnTable);
                GiveObject();
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