using System;
using UIScripts;
using UnityEngine;
using UnityEngine.Serialization;

public class Product : BaseObject
{
    [SerializeField] private ProductSO productData;
    [SerializeField] private ProductState productState;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ProductStateSO currentStateSO;
    
    [SerializeField] private bool isMergable;
    [SerializeField] private int range = 0;
    [SerializeField] ProductRangeUI productRangeUI;
    
    public void Start()
    {
        isMergable = productData.isMergable;
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
    
    public bool CanApplyAction(ProductAction action)
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
        if (productData.type != otherProduct.productData.type) return false;
        return otherProduct.range == range;
    }

    public override void CombineWith(BaseObject other)
    {
        Product otherProduct = (Product)other;
        range++;
        productRangeUI.UpdateRange(range);
        Destroy(otherProduct.gameObject);
        
        ProductExperienceManager.Instance.AddExperience(productData, 50);

        Debug.Log("Products combined -> upgraded");
    }

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
