using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingTool : BaseObject
{
    [SerializeField] private string type;
    [SerializeField] private bool hasObject;

    private Product[] _products;
    
    private void GetDirty()
    {
        
    }

    private void GetClean()
    {
        
    }
    
    public string GetObjectType()
    {
        return type;
    }
    
    public bool HasObject()
    {
        return hasObject;
    }
    
    public override bool CanMergeWith(BaseObject other)
    {
        return other is Product;
    }

    public override void Merge(BaseObject other)
    {
        Debug.Log("Add product to cooking tool");
    }
}
