namespace Tables
{
    public class CookingTable: BaseTable
    {
        private bool _hasObject;
        private enum CookingTableState
        {
            Idle,
            Cooking,
            Cooked,
            Burnt
        }
    }
}