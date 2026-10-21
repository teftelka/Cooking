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

    public event EventHandler OnContentsChanged;
    public event EventHandler OnStateChanged;

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
    }
    
    private void ApplyState(ProductStateSO state)
    {
        currentStateSO = state;
        productState = currentStateSO.state;
        spriteRenderer.sprite = state.sprite;
        
        if (productState != ProductState.Raw)
        {
            isMergable = false;
        }

        OnStateChanged?.Invoke(this, EventArgs.Empty);
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

    // A base takes a plain product, or everything cooked in a pan / pot (the cookware stays where it is)
    public override bool CanAccept(BaseObject other)
    {
        if (!IsBase) return false;

        var config = productData.baseConfig;
        if (config.acceptsInState == null || currentStateSO != config.acceptsInState) return false;

        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return false;
        if (contents.Count + incoming.Count > config.maxContents) return false;

        var fillings = new List<RecipeItem>();
        foreach (var ingredient in contents)
            fillings.Add(ingredient.ToRecipeItem());

        foreach (var product in incoming)
        {
            // Only plain products: a base with its own ingredients can't be put inside another base
            if (product == this || product.HasContents) return false;
            fillings.Add(product.GetRecipeKey());
        }

        return RecipeMatcher.FitsAnyRecipeWithBase(productData, fillings, config.recipes);
    }

    public override void Accept(BaseObject other)
    {
        var incoming = GetIncomingProducts(other);
        if (incoming == null || incoming.Count == 0) return;

        // Copy: emptying the cookware clears its list
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

        if (other is CookingTool cookware)
            cookware.EmptyContainer();

        OnContentsChanged?.Invoke(this, EventArgs.Empty);
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
    
    public void SetToParent(Transform parent)
    {
        //DisableImage();
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void DisableImage()
    {
        spriteRenderer.gameObject.SetActive(false);
        productRangeUI.SetActive(false);
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
