using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class ClearTable: BaseTable, IClickable
    {
        private void HandleSwap()
        {
            BaseObject productInHand = PlayerTest.Instance.GetProduct();
            
            if (productInHand.CanMergeWith(_product))
            {
                productInHand.Merge(_product);
                return;
            }

            Swap(productInHand, _product);
        }

        private void Swap(BaseObject productInHand, BaseObject productOnTable)
        {
            PlayerTest.Instance.ClearObject();
            PlayerTest.Instance.SetObject(productOnTable);
            productOnTable.RememberOrigin(productInHand.GetOrigin());
            
            _product = productInHand;
            _product.transform.position = spawnPosition.transform.position;
            _product.RememberOrigin(this);
        }

        public void OnClick()
        {
            bool playerHasObject = PlayerTest.Instance.HasObject();
            if (_hasObject)
            {
                if (playerHasObject)
                {
                    HandleSwap();
                    return;
                }

                PlayerTest.Instance.SetObject(TakeObject());
                return;
            }

            if (playerHasObject)
            {
                SetObjectOnTable(PlayerTest.Instance.GetProduct());
                PlayerTest.Instance.ClearObject();
            }
        }
    }
}