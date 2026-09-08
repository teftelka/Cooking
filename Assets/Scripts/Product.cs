using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
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
    [SerializeField] private List<Product> ingredients = new();
    [SerializeField] private Transform ingredientRoot;

    public IReadOnlyList<Product> Ingredients => ingredients.AsReadOnly();
    public event EventHandler OnIngredientsChanged;
    
    [SerializeField] private int range = 0;
    [SerializeField] ProductRangeUI productRangeUI;
    
    public void Awake()
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
    }
    
    public virtual bool CanApplyAction(ProductAction action)
    {
        return productData.CanApply(currentStateSO, action);
    }
    
    public bool ApplyAction(ProductAction action)
    {
        if (!productData.TryGetNextState(currentStateSO, action, out var next))
            return false;

        ApplyState(next);
        return true;
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

    // A food base stays a single product during transfers, but contributes its full recipe.
    public void AddRecipeItemsTo(List<RecipeItem> items)
    {
        items.Add(GetRecipeKey());
        foreach (var ingredient in ingredients)
            ingredient.AddRecipeItemsTo(items);
    }

    public override bool CanAccept(BaseObject other)
    {
        if (other == null || other == this) return false;
        if (other is Product product)
            return CanAddIngredients(new List<Product> { product });
        if (other is IProductContainer container)
            return CanAddIngredients(container.GetProducts());
        return false;
    }

    private bool CanAddIngredients(List<Product> incoming)
    {
        if (incoming.Count == 0 || ingredients.Count + incoming.Count > productData.ingredientCapacity)
            return false;

        var unique = new HashSet<Product>();
        foreach (var product in incoming)
        {
            if (product == null || product == this || !unique.Add(product) ||
                ingredients.Contains(product) || transform.IsChildOf(product.transform) ||
                !productData.CanReceiveIngredient(productState, product.GetRecipeKey()))
                return false;
        }
        return true;
    }

    public override void Accept(BaseObject other)
    {
        if (!CanAccept(other)) return;
        if (other is Product product)
        {
            AddIngredient(product);
        }
        else if (other is IProductContainer container)
        {
            foreach (var ingredient in new List<Product>(container.GetProducts()))
                AddIngredient(ingredient);
            container.EmptyContainer();
        }
        OnIngredientsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddIngredient(Product ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.SetToParent(ingredientRoot != null ? ingredientRoot : transform);
    }
    
    public void SetToParent(Transform parent)
    {
        //DisableImage();
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void DisableImage()
    {
        foreach (var ingredient in ingredients)
            ingredient.DisableImage();
        spriteRenderer.gameObject.SetActive(false);
        productRangeUI.SetActive(false);
    }
    
    public override bool CanCombineWith(BaseObject other)
    {
        if (!GetIsMergable()) return false;
        if (other is not Product otherProduct) return false;
        if (!otherProduct.GetIsMergable()) return false;
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
        return isMergable && ingredients.Count == 0;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
