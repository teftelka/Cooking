using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Product : BaseObject, IClickable
{
    [SerializeField] private ProductType type;
    [SerializeField] private int range;

   // public event EventHandler ProductDestroyed;

    public void Start()
    {
        range = 0;
    }

    public void OnClick()
    {
        
    }

    public ProductType GetObjectType()
    {
        return type;
    }
    
    public override bool CanMergeWith(BaseObject other)
    {
        return other switch
        {
            CookingTool => true,
            Product product => type == product.type,
            _ => false
        };
    }
    
    public override bool CanMergeWithSelf(BaseObject other)
    {
        if (other is not Product) return false;
        range++;
        Debug.Log("Merging two products");
        return true;
    }

    public override void Merge(BaseObject other)
    {
        Debug.Log("Merging with other object");
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
