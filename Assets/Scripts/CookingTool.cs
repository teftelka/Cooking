using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingTool : BaseObject
{
    [SerializeField] private string type;
    [SerializeField] private bool hasObject;
    [SerializeField] private List<Product> _products;
    
    public string GetObjectType()
    {
        return type;
    }
    
    public bool HasObject()
    {
        return hasObject;
    }
    
    public void CookRecipe()
    {
        
    }
    
    public override bool CanAccept(BaseObject other)
    {
        if (other is not Product product) return false;
        return product.GetProductState() == ProductState.Chopped;
    }

    public override void Accept(BaseObject other)
    {
        Product product = (Product)other;

        _products.Add(product);
        
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;

        Debug.Log("Product added to cooking tool");
    }
}
