using DefaultNamespace;
using Interfaces;
using Tables;

namespace Interaction
{
    /// <summary>
    /// The one place that decides what a click on a table does.
    /// Tables only describe their policy (CanPlace / CanTake / AllowSwap)
    /// and react through OnPlaced / OnTaken.
    ///
    /// Movement rule, used everywhere: when A accepts B,
    ///  - if B is a container (plate, cooking tool), only its contents move and B stays where it was;
    ///  - otherwise B itself moves into A and its previous place becomes free.
    /// </summary>
    public static class InteractionResolver
    {
        public static bool Resolve(BaseTable table, PlayerTest hand)
        {
            if (table == null || hand == null) return false;

            if (!hand.HasObject())
                return TryTake(table, hand);

            var inHand = hand.GetProduct();

            if (!table.HasObject)
                return TryPlace(table, hand, inHand);

            var onTable = table.ObjectOnTable;

            // 1. Put what is in hand into what is on the table
            if (onTable.CanAccept(inHand))
            {
                onTable.Accept(inHand);
                if (!(inHand is IProductContainer))
                    hand.HandleObjectGive();
                return true;
            }

            // 2. Put what is on the table into what is in hand
            if (table.CanTake() && inHand.CanAccept(onTable))
            {
                inHand.Accept(onTable);
                if (!(onTable is IProductContainer))
                    table.TakeObject();
                return true;
            }

            // 3. Swap
            if (table.AllowSwap && table.CanTake() && table.CanPlace(inHand))
            {
                Swap(table, hand, inHand);
                return true;
            }

            return false;
        }

        private static bool TryTake(BaseTable table, PlayerTest hand)
        {
            if (!table.HasObject || !table.CanTake()) return false;

            var taken = table.TakeObject();
            return hand.HandleObjectTake(taken);
        }

        private static bool TryPlace(BaseTable table, PlayerTest hand, BaseObject inHand)
        {
            if (!table.CanPlace(inHand)) return false;

            hand.HandleObjectGive();
            table.SetObjectOnTable(inHand);
            return true;
        }

        private static void Swap(BaseTable table, PlayerTest hand, BaseObject inHand)
        {
            // The object picked up remembers where the hand's object came from,
            // so right-click sends it there (same as before the refactor).
            var handOrigin = inHand.GetOrigin();

            hand.HandleObjectGive();
            var taken = table.TakeObject();
            table.SetObjectOnTable(inHand);

            hand.HandleObjectTake(taken);
            taken.RememberOrigin(handOrigin);
        }
    }
}
