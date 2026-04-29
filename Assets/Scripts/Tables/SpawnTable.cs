using System;
using DefaultNamespace;
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
                    return;
                var newProduct = Instantiate(productSO.prefab).GetComponent<BaseObject>();
                PlayerTest.Instance.HandleObjectTake(newProduct);
            }
        }

        private void SetProductSprite()
        {
            spawnerTableUI.SetProductSprite(productSO.icon);
        }
        
        public void SetProductSO(ProductSO product)
        {
            productSO = product;
            SetProductSprite();
        }

        public ProductSO GetProductSO()
        {
            return productSO;
        }
    }
}