using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class MergeTable: BaseTable, IClickable
    {
        [SerializeField] private GameObject firstPosition;
        [SerializeField] private GameObject secondPosition;
        [SerializeField] private MergeState mergingState;
        
        private List<Product> productsOnMerge = new List<Product>();
        
        
        private enum MergeState
        {
            Idle,
            Merging
        }
        
        public void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (playerProduct is Product product)
            {
                if (product.GetIsMergable())
                {
                    SetProductToMerge(product);
                    return;
                }
            }

            if (_hasObject)
            {
                HandleObjectGive();
            }
        }

        private void HandleObjectGive()
        {
            PlayerTest.Instance.HandleObjectTake(GiveObject());
        }

        private void SetProductToMerge(Product product)
        {
            
            if (productsOnMerge.Count == 0)
            {
                productsOnMerge.Add(product);
                product.transform.position = firstPosition.transform.position;
                PlayerTest.Instance.HandleObjectGive();
                return;
            }

            if (productsOnMerge[0].CanCombineWith(product))
            {
                productsOnMerge.Add(product);
                product.transform.position = secondPosition.transform.position;
                PlayerTest.Instance.HandleObjectGive();
                SpawnMergedProduct(product);
            }
        }
        
        private void SpawnMergedProduct(Product product)
        {
            var newProduct = Instantiate(product.GetProductData().prefab).GetComponent<Product>();
            SetObjectOnTable(newProduct);
            newProduct.SetRange(product.GetProductRange() + 1);
            
            ClearTable();
        }
        
        private void ClearTable()
        {
            foreach (var a in productsOnMerge)
            {
                a.DestroySelf();
            }
            productsOnMerge.Clear();
        }


        public override void SetObjectOnTable(BaseObject _object)
        {
            _hasObject = true;
            _objectOnTable = _object;
            _object.transform.position = spawnPosition.transform.position;
            _object.RememberOrigin(this);
        }

    }
}