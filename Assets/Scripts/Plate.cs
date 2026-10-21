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

    // Raised after any change of what is on the plate (including ingredients added to a base on it)
    public event EventHandler OnContentsChanged;

    public void GetDirty()
    {
        isDirty = true;
        //stainImage.gameObject.SetActive(true);
    }

    public override bool CanAccept(BaseObject other)
    {
        if (isDirty) return false;

        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return false;

        // 1. A base is on the plate: everything goes into the base, nothing else is accepted
        var baseOnPlate = GetBaseOnPlate();
        if (baseOnPlate != null)
            return baseOnPlate.CanAccept(other);

        // 2. A base arrives
        var incomingBase = FindBase(incoming);
        if (incomingBase != null)
        {
            if (incoming.Count != 1) return false;          // a base arrives alone
            if (other is Plate) return false;               // a base can't be moved from one plate to another
            return CanTakeBase(incomingBase);
        }

        // 3. No base involved: as before
        return CanAddProducts(incoming);
    }

    private bool CanTakeBase(Product incomingBase)
    {
        // Still being assembled: it will absorb whatever is already on the plate
        if (incomingBase.IsAcceptingIngredients)
            return incomingBase.CanAcceptProducts(_products);

        // Finished (e.g. baked pizza): only onto an empty plate and only as an exact recipe.
        // A burned base matches no recipe, so it is refused.
        if (_products.Count > 0) return false;
        var items = new List<RecipeItem>(incomingBase.GetRecipeItems());
        return RecipeMatcher.FindExactRecipe(items, recipeList) != null;
    }

    private Product GetBaseOnPlate()
    {
        return FindBase(_products);
    }

    private static Product FindBase(IReadOnlyList<Product> products)
    {
        foreach (var product in products)
        {
            if (product != null && product.IsBase) return product;
        }
        return null;
    }

    private static IReadOnlyList<Product> GetIncomingProducts(BaseObject other)
    {
        switch (other)
        {
            case Product product:
                return new[] { product };
            case IProductContainer container:
                return container.GetProducts();
            default:
                return null;
        }
    }

    public List<Product> GetProducts()
    {
        return _products;
    }

    public override void Accept(BaseObject other)
    {
        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return;

        // 1. A base is on the plate: the base takes the product / the cookware contents
        var baseOnPlate = GetBaseOnPlate();
        if (baseOnPlate != null)
        {
            baseOnPlate.Accept(other);
            TryCompleteRecipe();
            OnContentsChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Copy: emptying the container clears its list
        var arriving = new List<Product>(incoming);

        // 2. An unfinished base arrives on a plate with products: they become its ingredients
        var incomingBase = FindBase(arriving);
        if (incomingBase != null && incomingBase.IsAcceptingIngredients && _products.Count > 0)
        {
            var onPlate = new List<Product>(_products);
            _products.Clear();
            incomingBase.AcceptProducts(onPlate);
        }

        foreach (var product in arriving)
            AddProductToPlate(product);

        if (other is IProductContainer container)
            container.EmptyContainer();

        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void AddProductToPlate(Product product)
    {
        _products.Add(product);
        OnProductAdded?.Invoke(this, new OnProductAddedEventArgs { product = product });
        
        product.SetToParent(transform);
        TryCompleteRecipe();
    }
    
    private bool CanAddProducts(IReadOnlyList<Product> incomingProducts)
    {
        var futureItems = CollectRecipeItems(_products);
        futureItems.AddRange(CollectRecipeItems(incomingProducts));

        return RecipeMatcher.FitsAnyRecipe(futureItems, recipeList);
    }

    private static List<RecipeItem> CollectRecipeItems(IReadOnlyList<Product> products)
    {
        var items = new List<RecipeItem>();
        foreach (var product in products)
            items.AddRange(product.GetRecipeItems());
        return items;
    }
    
    // Re-evaluated after every change: the plate shows a dish only while its contents
    // exactly form a recipe. If a product is added and the set no longer matches,
    // the previous dish is removed so it can't be served by mistake.
    private void TryCompleteRecipe()
    {
        var items = CollectRecipeItems(_products);
        var recipe = RecipeMatcher.FindExactRecipe(items, recipeList);
        if (recipe == null)
        {
            ClearDish();
            return;
        }

        CreateDish(recipe);
        MakeModifiedRecipe(recipe, items);
    }

    private void ClearDish()
    {
        if (newRecipe != null)
        {
            Destroy(newRecipe);
            newRecipe = null;
        }
        completedRecipe = null;

        foreach (var p in _products)
            p.EnableImage();
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
    
    // The base on the plate, if it is the only thing there (e.g. a pizza ready for the oven)
    public Product GetSingleBase()
    {
        if (_products.Count != 1) return null;
        var product = _products[0];
        return product != null && product.IsBase ? product : null;
    }

    // Takes the single base off the plate without destroying it (it goes into cookware)
    public Product ReleaseSingleBase()
    {
        var baseProduct = GetSingleBase();
        if (baseProduct == null) return null;

        ClearDish();        // shows the base again if a dish picture was on
        EmptyContainer();   // clears the list and updates the icons
        return baseProduct;
    }

    public void EmptyContainer()
    {
        completedRecipe = null;
        Destroy(newRecipe);
        _products.Clear();
        
        OnEmptyPlate?.Invoke(this, EventArgs.Empty);
        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
