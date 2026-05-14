using System.Collections.Generic;
using DefaultNamespace;
using Interfaces;
using Managers;

namespace Tables
{
    public class OrderTable: BaseTable, IClickable
    {
        public void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (playerProduct)
            {
                if (playerProduct is Plate plate)
                {
                    HandleOrderComplete(plate);
                }
            }
        }
        
        private void HandleOrderComplete(Plate plate)
        {
            var recipe = plate.GetRecipeOnPlate();
            if (!recipe) return;

            var activeOrders = OrderManager.Instance.GetActiveOrders();
            
            foreach (var activeOrder in activeOrders)
            {
                if (!IsExactRecipe(recipe, activeOrder)) continue;
                OrderManager.Instance.CompleteOrder(activeOrder);
                plate.DestroySelf();
                break;

            }
        }

        private bool IsExactRecipe(RecipeSO recipeOnPlate, RecipeSO recipeOrder)
        {
            List<RecipeItem> remaining = new(recipeOrder.ingredients);
            foreach (var item in recipeOnPlate.ingredients)
            {
                int index = remaining.FindIndex(r =>
                    r.productSO == item.productSO &&
                    r.productState == item.productState &&
                    r.productLevel <= item.productLevel);

                if (index == -1)
                    return false;

                remaining.RemoveAt(index);
            }
            return true;
        }
    }
}