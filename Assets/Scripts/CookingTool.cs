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
    
    public override string GetObjectType()
    {
        return type;
    }
    
    public bool HasObject()
    {
        return hasObject;
    }
}
