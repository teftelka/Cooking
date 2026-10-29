using System;
using Interfaces;

namespace Tables
{
    public class RecycleBinTable: BaseTable
    {
        public override void Interact(IHand hand)
        {
            var heldObject = hand.HeldObject;
            if (heldObject)
            {
                HandleDestroy(heldObject, hand);
            }
        }

        private void HandleDestroy(BaseObject heldObject, IHand hand)
        {
            switch (heldObject)
            {
                case IProductContainer container:   // plate, cookware: emptied, stays in hand
                    container.DestroyContents();
                    break;
                case Product someProduct:
                    hand.Release();
                    Destroy(someProduct.gameObject);
                    break;
            }
        }
    }
}
