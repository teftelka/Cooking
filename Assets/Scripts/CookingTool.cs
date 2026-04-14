using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookingTool : BaseObject
{
    [SerializeField] private ProductAction toolAction;
    [SerializeField] private bool hasObject;
    [SerializeField] private List<Product> _products;
    [SerializeField] private int capacity = 3;
    
    public bool CanCook()
    {
        return _products.Any(product => product.CanApplyAction(toolAction));
    }
    
    public void CookRecipe()
    {
        foreach (var product in _products)
        {
            product.ApplyAction(toolAction);
        }
    }
    
    public override bool CanAccept(BaseObject other)
    {
        if (other is not Product product) return false;
        if (_products.Count >= capacity) return false;

        return product.CanApplyAction(toolAction);
    }

    public override void Accept(BaseObject other)
    {
        Product product = (Product)other;

        _products.Add(product);
        
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;
    }
}
