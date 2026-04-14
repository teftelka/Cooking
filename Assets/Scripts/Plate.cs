using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate: BaseObject
{
    [SerializeField] private bool hasObject;
    [SerializeField] private bool isDirty;
    [SerializeField] private List<Product> _products;
    private void GetDirty()
    {
        
    }

    private void GetClean()
    {
        
    }
    
    public void OnClick()
    {
        
    }
    
    public override bool CanAccept(BaseObject other)
    {
        return other is CookingTool;
    }

    public override void Accept(BaseObject other)
    {
        if (other is CookingTool cookingTool)
        {
            _products = cookingTool.GetProducts();
            foreach (var product in _products)
            {
                product.transform.SetParent(transform);
                product.transform.localPosition = Vector3.zero;
            }
            
            cookingTool.EmptyTool();
        }
        
        Debug.Log("Product added to plate");
    }
}
