namespace Tables
{
    // Plain counter: anything can be placed, taken, combined or swapped.
    public class ClearTable: BaseTable
    {
        public override bool AllowSwap => true;
    }
}
