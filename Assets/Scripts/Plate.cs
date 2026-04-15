using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate: BaseObject
{
    [SerializeField] private bool hasObject;
    [SerializeField] private bool isDirty;
    [SerializeField] private List<Product> _products;
    [SerializeField] private List<RecipeSO> possibleRecipes;
    private void GetDirty()
    {
        
    }

    private void GetClean()
    {
        
    }
    
    public void OnClick()
    {
        
    }
    
    public override bool CanAccept(BaseObject other)
    {
        switch (other)
        {
            case Product product:
                return CanAddProducts(new List<Product> { product });
            case CookingTool tool:
                return CanAddProducts(tool.GetProducts());
            default:
                return false;
        }
    }

    public override void Accept(BaseObject other)
    {
        if (other is CookingTool cookingTool)
        {
            foreach (var productInTool in cookingTool.GetProducts())
            {
                AddProductToPlate(productInTool);
            }
            cookingTool.EmptyTool();
            return;
        }

        if (other is Product product)
        {
            AddProductToPlate(product);
        }
        
        Debug.Log("Product added to plate");
    }
    
    private void AddProductToPlate(Product product)
    {
        _products.Add(product);
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;
    }
    
    private bool CanAddProducts(List<Product> incomingProducts)
    {
        List<RecipeItem> futureItems = new();

        foreach (var product in _products)
            futureItems.Add(product.GetRecipeKey());

        foreach (var product in incomingProducts)
            futureItems.Add(product.GetRecipeKey());
        
        foreach (var recipe in possibleRecipes)
        {
            if (IsSubsetOfRecipe(futureItems, recipe))
                return true;
        }

        return false;
    }
    
    private bool IsSubsetOfRecipe(List<RecipeItem> items, RecipeSO recipe)
    {
        List<RecipeItem> remaining = new(recipe.ingredients);
        foreach (var item in items)
        {
            int index = remaining.FindIndex(r =>
                r.productType == item.productType &&
                r.productState == item.productState);

            if (index == -1)
                return false;

            remaining.RemoveAt(index);
        }

        return true;
    }
    
    private void TryCompleteRecipe()
    {
        foreach (var recipe in possibleRecipes)
        {
            if (IsExactRecipe(recipe))
            {
                CreateDish(recipe);
                return;
            }
        }
    }
    
    private bool IsExactRecipe(RecipeSO recipe)
    {
        if (_products.Count != recipe.ingredients.Count)
            return false;

        return IsSubsetOfRecipe(_products.ConvertAll(p => p.GetRecipeKey()), recipe);
    }
    
    private void CreateDish(RecipeSO recipe)
    {
        foreach (var p in _products)
            Destroy(p.gameObject);

        _products.Clear();

        Instantiate(recipe.resultPrefab, transform.position, Quaternion.identity);
    }
}
