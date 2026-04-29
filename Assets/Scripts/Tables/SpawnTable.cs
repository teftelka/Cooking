using System;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class SpawnTable: MonoBehaviour, IClickable
    {
        [SerializeField] private GameObject productPrefab;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void OnClick()
        {
            if (!PlayerTest.Instance.HasObject())
            {
                var newProduct = Instantiate(productPrefab).GetComponent<BaseObject>();
                PlayerTest.Instance.HandleObjectTake(newProduct);
            }
        }

        private void SetProductSprite()
        {
            var productSprite = productPrefab.GetComponent<Product>().GetDefaultSprite();
            spriteRenderer.sprite = productSprite;
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