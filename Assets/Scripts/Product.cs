using System;
using System.Collections.Generic;
using Managers;
using Recipes;
using UIScripts;
using UnityEngine;
using UnityEngine.Serialization;

public class Product : BaseObject
{
    [SerializeField] private ProductSO productData;
    [SerializeField] private ProductState productState;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private ProductStateSO currentStateSO;
    private bool isMergable;
    
    [SerializeField] private int range = 0;
    [SerializeField] ProductRangeUI productRangeUI;

    // Ingredients put on this product. Used only when the product is a base (see ProductSO.baseConfig)
    [SerializeField] private List<IngredientData> contents = new List<IngredientData>();

    // Picture of the recipe's dish, shown by a base in its dish state (e.g. a baked pizza)
    private GameObject dishVisual;

    public event EventHandler OnContentsChanged;
    public event EventHandler OnStateChanged;
    public event EventHandler OnLevelChanged;
    public event EventHandler OnContainmentChanged;

    // True while the product lies inside a plate or cookware: their own icon panel shows it then
    public bool IsInsideContainer { get; private set; }

    public IReadOnlyList<IngredientData> Contents => contents;
    public bool IsBase => productData != null && productData.IsBase;
    public bool HasContents => contents.Count > 0;
    public ProductStateSO GetCurrentState() => currentStateSO;
    
    public void Start()
    {
        isMergable = productData.isMergable;
        currentStateSO = productData.startState;
        ApplyState(currentStateSO);
    }
    
    public ProductSO GetProductData()
    {
        return productData;
    }
    
    public int GetProductRange()
    {
        return range;
    }
    
    public void SetRange(int rarity)
    {
        range = rarity;
        productRangeUI.UpdateRange(rarity);
        OnLevelChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void ApplyState(ProductStateSO state)
    {
        currentStateSO = state;
        productState = currentStateSO.state;
        spriteRenderer.sprite = state.sprite;
        ApplyContentsSprite();
        
        if (productState != ProductState.Raw)
        {
            isMergable = false;
        }

        RefreshDishVisual();
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    // A base in its dish state shows the result prefab of the recipe it exactly forms,
    // so different pizzas look different right after baking. Otherwise the state sprite is used.
    private void RefreshDishVisual()
    {
        if (dishVisual != null)
        {
            Destroy(dishVisual);
            dishVisual = null;
        }

        bool showDish = false;
        if (IsBase)
        {
            var config = productData.baseConfig;
            if (config.dishState != null && currentStateSO == config.dishState)
            {
                var recipe = RecipeMatcher.FindExactRecipe(new List<RecipeItem>(GetRecipeItems()), config.recipes);
                if (recipe != null && recipe.resultPrefab != null)
                {
                    // Same way the plate places a dish: keep the prefab's world scale
                    dishVisual = Instantiate(recipe.resultPrefab, transform.position, Quaternion.identity);
                    dishVisual.transform.SetParent(transform, true);
                    dishVisual.transform.localPosition = Vector3.zero;
                    showDish = true;
                }
                else
                {
                    Debug.LogWarning($"{name}: no recipe matches this base in its dish state, the state sprite is shown", this);
                }
            }
        }

        spriteRenderer.enabled = !showDish;
    }
    
    public bool CanApplyAction(ProductAction action)
    {
        var rule = productData.FindRule(currentStateSO, action);
        return rule != null && (!rule.requiresRecipe || FormsRecipeAfter(rule));
    }
    
    public bool ApplyAction(ProductAction action)
    {
        var rule = productData.FindRule(currentStateSO, action);
        if (rule == null) return false;
        if (rule.requiresRecipe && !FormsRecipeAfter(rule)) return false;

        ApplyState(rule.toState);
        return true;
    }

    // After the transition, the base with its ingredients would exactly form a recipe
    private bool FormsRecipeAfter(ProductStateTransitionRule rule)
    {
        if (!IsBase || rule.toState == null) return false;

        var items = new List<RecipeItem>
        {
            new RecipeItem { productSO = productData, productState = rule.toState.state, productLevel = range }
        };
        foreach (var ingredient in contents)
            items.Add(ingredient.ToRecipeItem());

        return RecipeMatcher.FindExactRecipe(items, productData.baseConfig.recipes) != null;
    }

    // ---- Base: accepting ingredients ----

    // The base is in the state in which it takes ingredients (e.g. cut dough, raw bun)
    public bool IsAcceptingIngredients =>
        IsBase &&
        productData.baseConfig.acceptsInState != null &&
        currentStateSO == productData.baseConfig.acceptsInState;

    // A base takes a plain product, or everything cooked in a pan / pot (the cookware stays where it is)
    public override bool CanAccept(BaseObject other)
    {
        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return false;
        return CanAcceptProducts(incoming);
    }

    public override void Accept(BaseObject other)
    {
        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return;

        AcceptProducts(incoming);

        if (other is CookingTool cookware)
            cookware.EmptyContainer();
    }

    // Can these products become ingredients of this base? An empty list checks only the
    // base itself (used by a plate: "can this base still become a recipe?")
    public bool CanAcceptProducts(IReadOnlyList<Product> incoming)
    {
        if (!IsAcceptingIngredients) return false;

        var config = productData.baseConfig;
        if (contents.Count + incoming.Count > config.maxContents) return false;

        var fillings = new List<RecipeItem>();
        foreach (var ingredient in contents)
            fillings.Add(ingredient.ToRecipeItem());

        foreach (var product in incoming)
        {
            // Only plain products: a base with its own ingredients can't be put inside another base
            if (product == null || product == this || product.HasContents) return false;
            fillings.Add(product.GetRecipeKey());
        }

        return RecipeMatcher.FitsAnyRecipeWithBase(productData, fillings, config.recipes);
    }

    // Turns the products into ingredient records of this base and removes their objects
    public void AcceptProducts(IReadOnlyList<Product> incoming)
    {
        if (incoming == null || incoming.Count == 0) return;

        // Copy: the caller may clear its own list afterwards (cookware, plate)
        foreach (var product in new List<Product>(incoming))
        {
            contents.Add(new IngredientData
            {
                product = product.productData,
                state = product.currentStateSO,
                level = product.range
            });

            // The ingredient now lives only as data inside this base
            Destroy(product.gameObject);
        }

        ApplyContentsSprite();
        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }

    // While a base accepts ingredients, its picture depends on what is on it
    private void ApplyContentsSprite()
    {
        if (currentStateSO == null) return;

        Sprite sprite = null;
        if (IsAcceptingIngredients)
            sprite = productData.baseConfig.FindSpriteFor(contents);

        spriteRenderer.sprite = sprite != null ? sprite : currentStateSO.sprite;
    }

    private static IReadOnlyList<Product> GetIncomingProducts(BaseObject other)
    {
        switch (other)
        {
            case Product product:
                return new[] { product };
            case CookingTool cookware:
                return cookware.GetProducts();
            default:
                return null;
        }
    }
    
    public RecipeItem GetRecipeKey()
    {
        return new RecipeItem
        {
            productState = productState,
            productLevel = range,
            productSO = productData
        };
    }

    // Everything this product contributes to a recipe: itself plus the ingredients on it
    public IEnumerable<RecipeItem> GetRecipeItems()
    {
        yield return GetRecipeKey();
        foreach (var ingredient in contents)
            yield return ingredient.ToRecipeItem();
    }
    
    // Called by a plate or cookware when the product goes inside it
    public void SetToParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        SetInsideContainer(true);
    }

    // The product leaves a plate or cookware (e.g. a pizza taken out of the oven by hand)
    public void DetachFromContainer()
    {
        transform.SetParent(null);
        SetInsideContainer(false);
    }

    private void SetInsideContainer(bool inside)
    {
        if (IsInsideContainer == inside) return;
        IsInsideContainer = inside;
        OnContainmentChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DisableImage()
    {
        spriteRenderer.gameObject.SetActive(false);
        productRangeUI.SetActive(false);
        if (dishVisual != null) dishVisual.SetActive(false);
    }

    public void EnableImage()
    {
        spriteRenderer.gameObject.SetActive(true);
        if (dishVisual != null) dishVisual.SetActive(true);
        // The level badge is shown only for levels above zero
        if (range > 0)
            productRangeUI.SetActive(true);
    }
    
    public override bool CanCombineWith(BaseObject other)
    {
        if (!isMergable) return false;
        if (other is not Product otherProduct) return false;
        if (HasContents || otherProduct.HasContents) return false;
        if (productData != otherProduct.productData) return false;
        return otherProduct.range == range;
    }

    /*public override void CombineWith(BaseObject other)
    {
        Product otherProduct = (Product)other;
        range++;
        productRangeUI.UpdateRange(range);
        Destroy(otherProduct.gameObject);
        
        ProductExperienceManager.Instance.AddExperience(productData, 50);

        Debug.Log("Products combined -> upgraded");
    }*/

    public Sprite GetDefaultSprite()
    {
        return productData.icon;
    }
    
    public bool GetIsMergable()
    {
        return isMergable;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
