using System;
using System.Collections.Generic;
using DefaultNamespace;
using Interfaces;
using Managers;
using UnityEngine;

namespace Tables
{
    public class MergeTable: BaseTable, IClickable
    {
        [SerializeField] private GameObject firstPosition;
        [SerializeField] private GameObject secondPosition;
        [SerializeField] private MergeState mergingState;
        
        [SerializeField] private float mergingTime = 10f;
        [SerializeField] private float _timer;
        
        [SerializeField] private List<Product> productsOnMerge = new List<Product>();
        
        public EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
        public class OnProgressChangedEventArgs {
            public float progress; 
        }
        
        private enum MergeState
        {
            Idle,
            Merging
        }
        
        public void OnClick()
        {
            if (mergingState == MergeState.Merging) return;
            
            var playerProduct = PlayerTest.Instance.GetProduct();
            if (playerProduct is Product product && product.GetIsMergable())
            {
                if (_hasObject) return;
                SetProductToMerge(product);
                return;
            }

            if (_hasObject)
            {
                HandleObjectGive();
                return;
            }

            if (productsOnMerge.Count > 0)
            {
                _objectOnTable = productsOnMerge[0];
                HandleObjectGive();
            }
        }
        
        private void Update()
        {
            if (mergingState != MergeState.Merging) return;
            
            _timer -= Time.deltaTime;
            OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { progress = 1 - (_timer / mergingTime) });
            
            if (_timer > 0f) return;
            
            mergingState = MergeState.Idle;
            SpawnMergedProduct(productsOnMerge[0]);
        }

        private void HandleObjectGive()
        {
            PlayerTest.Instance.HandleObjectTake(GiveObject());
            productsOnMerge.Clear();
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
                
                mergingState = MergeState.Merging;
                _timer = mergingTime;
            }
        }
        
        private void SpawnMergedProduct(Product product)
        {
            var newProduct = Instantiate(product.GetProductData().prefab).GetComponent<Product>();
            SetObjectOnTable(newProduct);
            newProduct.SetRange(product.GetProductRange() + 1);
            ProductExperienceManager.Instance.AddExperience(newProduct.GetProductData(), 50);
            ClearTable();
        }
        
        private void ClearTable()
        {
            foreach (var mergedProduct in productsOnMerge)
            {
                mergedProduct.DestroySelf();
            }
            //productsOnMerge.Clear();
        }
    }
}