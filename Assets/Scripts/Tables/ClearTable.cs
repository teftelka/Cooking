using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class ClearTable: BaseTable, IClickable
    {
        private void HandleMerge(BaseObject productInHand, BaseObject productOnTable)
        {
            if (productOnTable.CanCombineWith(productInHand))
            {
                productOnTable.CombineWith(productInHand);
                PlayerTest.Instance.HandleObjectGive();
                return;
            }
            
            if (productOnTable.CanAccept(productInHand)) // && productInHand.CanBeAcceptedBy(productOnTable))
            {
                productOnTable.Accept(productInHand);
                PlayerTest.Instance.HandleObjectGive();
                return;
            }

            if (productInHand.CanAccept(productOnTable)) // && productOnTable.CanBeAcceptedBy(productInHand))
            {
                productInHand.Accept(productOnTable);
                _product = null;
                _hasObject = false;
                return;
            }
            
            Swap(productInHand, productOnTable);
        }

        private void Swap(BaseObject productInHand, BaseObject productOnTable)
        {
            PlayerTest.Instance.HandleObjectGive();
            PlayerTest.Instance.HandleObjectTake(productOnTable);
            productOnTable.RememberOrigin(productInHand.GetOrigin());
            
            _product = productInHand;
            _product.transform.position = spawnPosition.transform.position;
            _product.RememberOrigin(this);
        }

        public void OnClick()
        {
            var productInHand = PlayerTest.Instance.GetProduct();
            if (_hasObject)
            {
                if (productInHand)
                {
                    HandleMerge(productInHand, _product);
                    return;
                }

                PlayerTest.Instance.HandleObjectTake(GiveObject());
                return;
            }

            if (productInHand)
            {
                SetObjectOnTable(productInHand);
                PlayerTest.Instance.HandleObjectGive();
            }
        }
    }
}