using DefaultNamespace;

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
                if (activeOrder == recipe)
                {
                    OrderManager.Instance.CompleteOrder(activeOrder);
                    plate.FinishOrder();
                    return;
                }
            }
        }
    }
}