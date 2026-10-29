using System;
using Interfaces;
using Managers;
using UIScripts;
using UnityEngine;

namespace Tables
{
    public class SpawnTable: MonoBehaviour, IInteractable
    {
        [SerializeField] private ProductSO productSO;
        [SerializeField] private SpawnerTableUI spawnerTableUI;
        
        private void Start()
        {
            ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
            ProductExperienceManager.Instance.OnLevelChanged += HandleLevelChanged;
            RefreshAmount();
        }

        private void OnDestroy()
        {
            // Spawners are destroyed and recreated on every completed order,
            // so we must unsubscribe from long-lived managers here.
            // Explicit != null (not ?.) respects Unity's destroyed-object check.
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourceChanged -= HandleResourceChanged;
            if (ProductExperienceManager.Instance != null)
                ProductExperienceManager.Instance.OnLevelChanged -= HandleLevelChanged;
        }

        private void HandleLevelChanged(object sender, ProductExperienceManager.OnLevelChangedEventArgs e)
        {
            if (productSO != e.productSO) return;
            RefreshAmount();
        }

        private void HandleResourceChanged(object sender, ResourceManager.OnResourceChangedEventArgs e)
        {
            if (productSO != e.productSO) return;
            RefreshAmount();
        }

        // Shows the stock of the level this spawner actually spends from.
        private void RefreshAmount()
        {
            if (!productSO) return;
            int level = ProductExperienceManager.Instance.GetLevel(productSO);
            spawnerTableUI.UpdateProductAmount(ResourceManager.Instance.GetAmount(productSO, level));
        }

        public void Interact(IHand hand)
        {
            if (!hand.HasObject)
            {
                int productRange = ProductExperienceManager.Instance.GetLevel(productSO);
                if (!ResourceManager.Instance.TrySpendResource(productSO, productRange, 1))
                {
                    if (!ScoreManager.Instance.TrySpendMoney(productSO.price)) return;
                }
                    
                var newProduct = Instantiate(productSO.prefab).GetComponent<BaseObject>();
                newProduct.GetComponent<Product>().SetRange(productRange);
                hand.TryTake(newProduct);
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