using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Recipes;
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
        var futureItems = CollectRecipeItems(_products);
        futureItems.AddRange(CollectRecipeItems(incomingProducts));

        return RecipeMatcher.FitsAnyRecipe(futureItems, recipeList);
    }

    private static List<RecipeItem> CollectRecipeItems(List<Product> products)
    {
        var items = new List<RecipeItem>();
        foreach (var product in products)
            items.AddRange(product.GetRecipeItems());
        return items;
    }
    
    private void TryCompleteRecipe()
    {
        var items = CollectRecipeItems(_products);
        var recipe = RecipeMatcher.FindExactRecipe(items, recipeList);
        if (recipe == null) return;

        CreateDish(recipe);
        MakeModifiedRecipe(recipe, items);
    }
    
    private void MakeModifiedRecipe(RecipeSO recipe, List<RecipeItem> recipeItems)
    {
        var modifiedRecipeSO = ScriptableObject.CreateInstance<RecipeSO>();
        modifiedRecipeSO.ingredients = recipeItems;
        modifiedRecipeSO.resultPrefab = recipe.resultPrefab;
        modifiedRecipeSO.recipeName = recipe.recipeName;
        
        completedRecipe = modifiedRecipeSO;
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
