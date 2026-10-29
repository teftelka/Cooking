using System.Collections.Generic;
using Interfaces;
using Managers;
using Recipes;

namespace Tables
{
    public class OrderTable: BaseTable
    {
        public override void Interact(IHand hand)
        {
            var held = hand.HeldObject;
            if (held == null) return;   // an empty hand is not an attempt to serve

            if (held is not Plate plate)
            {
                Reject(OrderRejectReason.NeedsPlate, null);
                return;
            }

            HandleOrderComplete(plate, hand);
        }
        
        private void HandleOrderComplete(Plate plate, IHand hand)
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

            // Of all orders this dish satisfies, take the one with the highest requirements,
            // so a strong dish is not wasted on an easy order. On a tie, the oldest order
            bool sameRecipeOrdered = false;
            Dish bestOrder = null;
            int bestLevels = -1;
            foreach (var activeOrder in OrderManager.Instance.GetActiveOrders())
            {
                if (activeOrder.recipe != dish.recipe) continue;
                sameRecipeOrdered = true;

                if (!RecipeMatcher.SatisfiesOrder(dish.ingredients, activeOrder.ingredients)) continue;

                int levels = SumOfLevels(activeOrder);
                if (levels <= bestLevels) continue;
                bestLevels = levels;
                bestOrder = activeOrder;
            }

            if (bestOrder != null)
            {
                OrderManager.Instance.CompleteOrder(bestOrder);
                hand.Release();
                plate.DestroySelf();
                return;
            }

            Reject(sameRecipeOrdered ? OrderRejectReason.LevelTooLow : OrderRejectReason.NoSuchOrder, dish);
        }

        private static int SumOfLevels(Dish order)
        {
            int sum = 0;
            foreach (var item in order.ingredients)
                sum += item.productLevel;
            return sum;
        }

        private void Reject(OrderRejectReason reason, Dish dish)
        {
            OrderManager.Instance.RejectOrder(reason, dish, this);
        }
    }
}
