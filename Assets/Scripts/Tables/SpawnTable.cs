using System;
using DefaultNamespace;
using Interfaces;
using Managers;
using UIScripts;
using UnityEngine;

namespace Tables
{
    public class SpawnTable: MonoBehaviour, IClickable
    {
        [SerializeField] private ProductSO productSO;
        [SerializeField] private SpawnerTableUI spawnerTableUI;
        
        private void Start()
        {
            ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
            var initialAmount = ResourceManager.Instance.GetAmount(productSO);
            spawnerTableUI.UpdateProductAmount(initialAmount);
        }

        private void HandleResourceChanged(object sender, ResourceManager.OnResourceChangedEventArgs e)
        {
            if (productSO != e.productSO) return;
            spawnerTableUI.UpdateProductAmount(e.newAmount);
        }

        public void OnClick()
        {
            if (!PlayerTest.Instance.HasObject())
            {
                if (!ResourceManager.Instance.TrySpend(productSO, 1))
                {
                    if (!ScoreManager.Instance.TrySpendMoney(productSO.price)) return;
                }
                    
                var newProduct = Instantiate(productSO.prefab).GetComponent<BaseObject>();
                PlayerTest.Instance.HandleObjectTake(newProduct);
            }
        }

        private void ArrangeSpawner()
        {
            spawnerTableUI.SetSpawnerInfo(productSO.icon, productSO.price);
        }
        
        public void SetProductSO(ProductSO product)
        {
            productSO = product;
            ArrangeSpawner();
        }

        public ProductSO GetProductSO()
        {
            return productSO;
        }
    }
}