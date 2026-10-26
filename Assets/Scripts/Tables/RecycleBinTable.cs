using System;
using DefaultNamespace;
using Interfaces;

namespace Tables
{
    public class RecycleBinTable: BaseTable
    {
        public override void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (playerProduct)
            {
                HandleDestroy(playerProduct);
            }
        }

        private void HandleDestroy(BaseObject playerProduct)
        {
            switch (playerProduct)
            {
                case IProductContainer container:   // plate, cookware: emptied, stays in hand
                    container.DestroyContents();
                    break;
                case Product someProduct:
                    PlayerTest.Instance.HandleObjectGive();
                    Destroy(someProduct.gameObject);
                    break;
            }
        }
    }
}