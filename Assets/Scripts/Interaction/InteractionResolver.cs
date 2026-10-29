using Interfaces;
using Tables;

namespace Interaction
{
    /// <summary>
    /// The one place that decides what using a hand on a table does
    /// (the player's hand now, helpers' hands later).
    /// Tables only describe their policy (CanPlace / CanTake / AllowSwap)
    /// and react through OnPlaced / OnTaken.
    ///
    /// Movement rule, used everywhere: when A accepts B,
    ///  - if B is a container (plate, cooking tool), only its contents move and B stays where it was;
    ///  - otherwise B itself moves into A and its previous place becomes free.
    /// </summary>
    public static class InteractionResolver
    {
        public static bool Resolve(BaseTable table, IHand hand)
        {
            if (table == null || hand == null) return false;

            if (!hand.HasObject)
                return TryTake(table, hand);

            var inHand = hand.HeldObject;

            if (!table.HasObject)
                return TryPlace(table, hand, inHand);

            var onTable = table.ObjectOnTable;

            // 1. Put what is in hand into what is on the table
            if (onTable.CanAccept(inHand))
            {
                onTable.Accept(inHand);
                if (!(inHand is IProductContainer))
                    hand.Release();
                return true;
            }

            // 2. Put what is on the table into what is in hand.
            // CanTake matters only if the object leaves the table: from a container
            // (pan, fixed baking sheet) only its contents move and the container stays
            if (inHand.CanAccept(onTable) && (onTable is IProductContainer || table.CanTake()))
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

        private static bool TryTake(BaseTable table, IHand hand)
        {
            var taken = table.TakeForEmptyHand();
            if (taken == null) return false;
            return hand.TryTake(taken);
        }

        private static bool TryPlace(BaseTable table, IHand hand, BaseObject inHand)
        {
            if (!table.CanPlace(inHand)) return false;

            hand.Release();
            table.SetObjectOnTable(inHand);
            return true;
        }

        private static void Swap(BaseTable table, IHand hand, BaseObject inHand)
        {
            // The object picked up remembers where the hand's object came from,
            // so right-click sends it there (same as before the refactor).
            var handOrigin = inHand.GetOrigin();

            hand.Release();
            var taken = table.TakeObject();
            table.SetObjectOnTable(inHand);

            hand.TryTake(taken);
            taken.RememberOrigin(handOrigin);
        }
    }
}
