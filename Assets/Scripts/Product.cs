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


   // public event EventHandler ProductDestroyed;

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
    
    public override bool CanBeAcceptedBy(BaseObject other)
    {
        return other is CookingTool or Plate;
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
