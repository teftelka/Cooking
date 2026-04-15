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
                if (productOnTable is Plate && productInHand is CookingTool) return;
                PlayerTest.Instance.HandleObjectGive();
                
                return;
            }

            if (productInHand.CanAccept(productOnTable)) // && productOnTable.CanBeAcceptedBy(productInHand))
            {
                productInHand.Accept(productOnTable);
                if (productInHand is Plate && productOnTable is CookingTool) return;
                _objectOnTable = null;
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
            
            _objectOnTable = productInHand;
            _objectOnTable.transform.position = spawnPosition.transform.position;
            _objectOnTable.RememberOrigin(this);
        }

        public void OnClick()
        {
            var productInHand = PlayerTest.Instance.GetProduct();
            if (_hasObject)
            {
                if (productInHand)
                {
                    HandleMerge(productInHand, _objectOnTable);
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