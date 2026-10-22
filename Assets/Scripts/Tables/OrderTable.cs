using System.Collections.Generic;
using DefaultNamespace;
using Managers;
using Recipes;

namespace Tables
{
    public class OrderTable: BaseTable
    {
        public override void OnClick()
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
            var dish = plate.GetDishOnPlate();
            if (dish == null) return;

            var activeOrders = OrderManager.Instance.GetActiveOrders();
            
            foreach (var activeOrder in activeOrders)
            {
                if (!RecipeMatcher.SatisfiesOrder(dish.ingredients, activeOrder.ingredients)) continue;
                OrderManager.Instance.CompleteOrder(activeOrder);
                plate.DestroySelf();
                break;

            }
        }
    }
}