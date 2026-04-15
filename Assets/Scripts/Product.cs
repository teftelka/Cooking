using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Product : BaseObject, IClickable
{
    [SerializeField] private ProductType type;
    [SerializeField] private int range;
    [SerializeField] private ProductState productState;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject image;
    [SerializeField] protected ProductStateSO currentState;
    

    public void Start()
    {
        spriteRenderer = image.GetComponent<SpriteRenderer>();
        ApplyState(currentState);
        range = 0;
    }

    public void OnClick()
    {
        
    }
    
    /*private ProductState GetProductState()
    {
        return productState;
    }
    
    private ProductType GetObjectType()
    {
        return type;
    }*/
    
    private void ApplyState(ProductStateSO state)
    {
        currentState = state;
        productState = currentState.state;
        spriteRenderer.sprite = state.sprite;
        spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }
    
    public bool CanApplyAction(ProductAction action)
    {
        return currentState.HasTransition(action);
    }
    
    public bool ApplyAction(ProductAction action)
    {
        if (!currentState.TryGetTransition(action, out var nextState))
            return false;

        ApplyState(nextState);
        return true;
    }
    
    public RecipeItem GetRecipeKey()
    {
        return new RecipeItem
        {
            productType = type,
            productState = productState
        };
    }
    
    public override bool CanCombineWith(BaseObject other)
    {
        if (other is not Product otherProduct) return false;
        if (type != otherProduct.type) return false;
        if (otherProduct.productState != ProductState.Raw) return false;
        if (productState != ProductState.Raw) return false;
        
        return true;
    }

    public override void CombineWith(BaseObject other)
    {
        Product otherProduct = (Product)other;
        range++;
        Destroy(otherProduct.gameObject);

        Debug.Log("Products combined -> upgraded");
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
