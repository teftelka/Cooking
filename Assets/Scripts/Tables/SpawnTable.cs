using System;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class SpawnTable: MonoBehaviour, IClickable
    {
        [SerializeField] private GameObject productPrefab;
        [SerializeField] private SpawnerTableUI spawnerTableUI;
        
        public void OnClick()
        {
            if (!PlayerTest.Instance.HasObject())
            {
                if (!ResourceManager.Instance.TrySpend(productPrefab, 1))
                    return;
                var newProduct = Instantiate(productPrefab).GetComponent<BaseObject>();
                PlayerTest.Instance.HandleObjectTake(newProduct);
            }
        }

        private void SetProductSprite()
        {
            var productSprite = productPrefab.GetComponent<Product>().GetDefaultSprite();
            spawnerTableUI.SetImage(productSprite);
        }
        
        public void SetProductPrefab(GameObject prefab)
        {
            productPrefab = prefab;
            SetProductSprite();
        }

        public GameObject GetProductPrefab()
        {
            return productPrefab;
        }
    }
}