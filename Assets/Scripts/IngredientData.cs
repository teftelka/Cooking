// An ingredient stored inside a base product (pizza dough, nori, bun).
// Once put on a base, an ingredient is no longer a separate object: the base keeps this record.
[System.Serializable]
public struct IngredientData
{
    public ProductSO product;
    public ProductStateSO state;
    public int level;

    public RecipeItem ToRecipeItem()
    {
        return new RecipeItem
        {
            productSO = product,
            productState = state != null ? state.state : default,
            productLevel = level
        };
    }
}
