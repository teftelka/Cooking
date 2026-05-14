using System;
using DefaultNamespace;
using Interfaces;

namespace Tables
{
    public class RecycleBinTable: BaseTable, IClickable
    {
        public void OnClick()
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
                case CookingTool cookingTool:
                    cookingTool.DestroyAllProducts();
                    return;
                case Product someProduct:
                    PlayerTest.Instance.HandleObjectGive();
                    Destroy(someProduct.gameObject);
                    break;
                case Plate plate:
                    plate.ResetPlate();
                    break;
            }
        }
    }
}