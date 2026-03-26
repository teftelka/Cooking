using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate: BaseObject
{
    private void GetDirty()
    {
        
    }

    private void GetClean()
    {
        
    }
    
    public void OnClick()
    {
        
    }
    
    public override bool CanMergeWith(BaseObject other)
    {
        return other is Product;
    }

    public override void Merge(BaseObject other)
    {
        Debug.Log("Add product to plate");
    }
}
