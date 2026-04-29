using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Products
{
    [CreateAssetMenu(fileName = "ProductList", menuName = "Scriptable Objects/ProductList")]
    public class ProductList : ScriptableObject
    {
        public List<ProductSO>  allRawProducts;
    }
}
