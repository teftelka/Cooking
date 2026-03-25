using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class ClearTable: BaseTable, IClickable
    {
        [SerializeField] private bool _hasObject;
        [SerializeField] private BaseObject _product;


        private void SetObjectOnTable()
        {
            _hasObject = true;
            _product = PlayerTest.Instance.GetProduct();
            _product.transform.position = spawnPosition.transform.position;
            PlayerTest.Instance.ClearObject();
        }

        private void SendObjectToHand()
        {
            PlayerTest.Instance.SetObject(_product);
            _hasObject = false;
            _product = null;
        }

        private void HandleSwap()
        {
            BaseObject productInHand = PlayerTest.Instance.GetProduct();

            string productInHandType = productInHand.GetObjectType();
            string productOnTableType = _product.GetObjectType();
            
            if (productInHandType == productOnTableType)
            {
                HandleMerge();
                return;
            }

            Swap(productInHand, _product);
        }

        private void Swap(BaseObject productInHand, BaseObject productOnTable)
        {
            PlayerTest.Instance.ClearObject();
            PlayerTest.Instance.SetObject(productOnTable);
            
            _product = productInHand;
            _product.transform.position = spawnPosition.transform.position;
        }

        private void HandleMerge()
        {
            Debug.LogWarning("Merging product");
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

                SendObjectToHand();
                return;
            }
            
            SetObjectOnTable();
        }
    }
}