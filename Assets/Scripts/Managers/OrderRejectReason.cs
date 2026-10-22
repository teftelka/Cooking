namespace Managers
{
    // Why the order counter refused a plate
    public enum OrderRejectReason
    {
        NeedsPlate,     // something other than a plate in hand (e.g. a pizza)
        EmptyPlate,     // nothing on the plate
        NotFinished,    // products on the plate, but no finished dish (e.g. unbaked pizza)
        NoSuchOrder,    // a finished dish nobody ordered right now
        LevelTooLow     // such an order exists, but an ingredient level is below the required one
    }

    // Localization keys of the messages, one per reason
    public static class OrderRejectReasonKeys
    {
        public const string NeedsPlate = "order.reject.needs_plate";
        public const string EmptyPlate = "order.reject.empty_plate";
        public const string NotFinished = "order.reject.not_finished";
        public const string NoSuchOrder = "order.reject.no_such_order";
        public const string LevelTooLow = "order.reject.level_too_low";

        public static string Get(OrderRejectReason reason)
        {
            switch (reason)
            {
                case OrderRejectReason.NeedsPlate: return NeedsPlate;
                case OrderRejectReason.EmptyPlate: return EmptyPlate;
                case OrderRejectReason.NotFinished: return NotFinished;
                case OrderRejectReason.NoSuchOrder: return NoSuchOrder;
                case OrderRejectReason.LevelTooLow: return LevelTooLow;
                default: return reason.ToString();
            }
        }
    }
}
