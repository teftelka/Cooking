using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Product : BaseObject, IClickable
{
    [SerializeField] private ProductType type;
    
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

    public override void Merge(BaseObject other)
    {
        Debug.Log("Merging two products");
    }
}
