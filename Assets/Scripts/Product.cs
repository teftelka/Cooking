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
    

    public void Start()
    {
        spriteRenderer = image.GetComponent<SpriteRenderer>();
        ApplyState(currentState);
        range = 0;
    }

    public void OnClick()
    {
        
    }

    public ProductType GetObjectType()
    {
        return type;
    }
    
    public ProductState GetProductState()
    {
        return productState;
    }
    
    private void ApplyState(ProductStateSO state)
    {
        currentState = state;
        productState = currentState.state;
        spriteRenderer.sprite = state.sprite;
        spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }
    
    public void Cut()
    {
        if (productState != ProductState.Raw) return;
        ApplyState(currentState.nextStateAsset);
    }
    
    public void Cook()
    {
        if (productState != ProductState.Chopped) return;
        ApplyState(currentState.nextStateAsset);
    }
    
    
    /*public override bool CanBeAcceptedBy(BaseObject other)
    {
        return other is CookingTool or Plate;
    }*/
    
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
