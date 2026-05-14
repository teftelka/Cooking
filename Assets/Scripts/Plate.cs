using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class Plate: BaseObject, IProductContainer
{
    [SerializeField] private bool hasObject;
    [SerializeField] private bool isDirty;
    [SerializeField] private List<Product> _products;
    [SerializeField] private RecipeListSO recipeList;
    [SerializeField] private RecipeSO completedRecipe;
    //[SerializeField] private SpriteRenderer stainImage;
    private GameObject newRecipe;
    
    public EventHandler<OnProductAddedEventArgs> OnProductAdded;
    public class OnProductAddedEventArgs : EventArgs
    {
        public Product product;
    }
    
    public EventHandler OnEmptyPlate;

    public void GetDirty()
    {
        isDirty = true;
        //stainImage.gameObject.SetActive(true);
    }

    public override bool CanAccept(BaseObject other)
    {
        if (isDirty) return false;
        switch (other)
        {
            case Product product:
                return CanAddProducts(new List<Product> { product });
            case IProductContainer container:
                if (container.GetProducts().Count == 0) return false;
                return CanAddProducts(container.GetProducts());
            default:
                return false;
        }
    }

    public List<Product> GetProducts()
    {
        return _products;
    }

    public override void Accept(BaseObject other)
    {
        if (other is Product product)
        {
            AddProductToPlate(product);
            return;
        }

        if (other is IProductContainer container)
        {
            foreach (var productInContainer in container.GetProducts())
            {
                AddProductToPlate(productInContainer);
            }
            container.EmptyContainer();
        }
        Debug.Log("Product added to plate");
    }
    
    private void AddProductToPlate(Product product)
    {
        _products.Add(product);
        OnProductAdded?.Invoke(this, new OnProductAddedEventArgs { product = product });
        
        product.SetToParent(transform);
        TryCompleteRecipe();
    }
    
    private bool CanAddProducts(List<Product> incomingProducts)
    {
        List<RecipeItem> futureItems = new();

        foreach (var product in _products)
            futureItems.Add(product.GetRecipeKey());

        foreach (var product in incomingProducts)
            futureItems.Add(product.GetRecipeKey());
        
        foreach (var recipe in recipeList.allRecipes)
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
                r.productSO == item.productSO &&
                r.productState == item.productState);

            if (index == -1)
                return false;

            remaining.RemoveAt(index);
        }

        return true;
    }
    
    private void TryCompleteRecipe()
    {
        foreach (var recipe in recipeList.allRecipes)
        {
            if (IsExactRecipe(recipe))
            {
                CreateDish(recipe);
                MakeModifiedRecipe(recipe);
                return;
            }
        }
    }
    
    private void MakeModifiedRecipe(RecipeSO recipe)
    {
        var recipeItems = new List<RecipeItem>();
        
        foreach (var product in _products)
        {
            recipeItems.Add(product.GetRecipeKey());
        }
        
        var modifiedRecipeSO = ScriptableObject.CreateInstance<RecipeSO>();
        modifiedRecipeSO.ingredients = recipeItems;
        modifiedRecipeSO.resultPrefab = recipe.resultPrefab;
        modifiedRecipeSO.recipeName = recipe.recipeName;
        
        completedRecipe = modifiedRecipeSO;
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
            p.DisableImage();

        if (newRecipe)
        {
            Destroy(newRecipe);
        }
        
        newRecipe = Instantiate(recipe.resultPrefab, transform.position, Quaternion.identity);
        newRecipe.transform.SetParent(transform);
        newRecipe.transform.localPosition = Vector3.zero;
    }
    
    public RecipeSO GetRecipeOnPlate()
    {
        return completedRecipe;
    }

    public void ResetPlate()
    {
        foreach (var p in _products)
            Destroy(p.gameObject);
        
        EmptyContainer();
    }
    
    public void EmptyContainer()
    {
        completedRecipe = null;
        Destroy(newRecipe);
        _products.Clear();
        
        OnEmptyPlate?.Invoke(this, EventArgs.Empty);
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
