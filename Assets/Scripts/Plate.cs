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
    
    /*public override bool CanAccept(BaseObject other)
    {
        return other is Product;
    }

    public override void Accept(BaseObject other)
    {
        Product product = (Product)other;

        _products.Add(product);
        
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;

        Debug.Log("Product added to cooking tool");
    }*/
}
