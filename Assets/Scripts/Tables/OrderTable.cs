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
            var held = PlayerTest.Instance.GetProduct();
            if (held == null) return;   // an empty hand is not an attempt to serve

            if (held is not Plate plate)
            {
                Reject(OrderRejectReason.NeedsPlate, null);
                return;
            }

            HandleOrderComplete(plate);
        }
        
        private void HandleOrderComplete(Plate plate)
        {
            if (plate.GetProducts().Count == 0)
            {
                Reject(OrderRejectReason.EmptyPlate, null);
                return;
            }

            var dish = plate.GetDishOnPlate();
            if (dish == null)
            {
                Reject(OrderRejectReason.NotFinished, null);
                return;
            }

            bool sameRecipeOrdered = false;
            foreach (var activeOrder in OrderManager.Instance.GetActiveOrders())
            {
                if (activeOrder.recipe != dish.recipe) continue;
                sameRecipeOrdered = true;

                if (!RecipeMatcher.SatisfiesOrder(dish.ingredients, activeOrder.ingredients)) continue;

                OrderManager.Instance.CompleteOrder(activeOrder);
                plate.DestroySelf();
                return;
            }

            Reject(sameRecipeOrdered ? OrderRejectReason.LevelTooLow : OrderRejectReason.NoSuchOrder, dish);
        }

        private void Reject(OrderRejectReason reason, Dish dish)
        {
            OrderManager.Instance.RejectOrder(reason, dish, this);
        }
    }
}