using UnityEngine;

[System.Serializable]
public struct RecipeItem
{
    public ProductType productType;
    public ProductState productState;
    public int productLevel;
    public GameObject productPrefab;
}