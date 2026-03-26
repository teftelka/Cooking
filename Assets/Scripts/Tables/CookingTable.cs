namespace Tables
{
    public class CookingTable: BaseTable
    {
        private enum CookingTableState
        {
            Idle,
            Cooking,
            Cooked,
            Burnt
        }
    }
}