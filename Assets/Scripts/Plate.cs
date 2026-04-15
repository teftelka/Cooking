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
        if (other is Product)
        {
            return true;
        }
        return other is CookingTool;
    }

    public override void Accept(BaseObject other)
    {
        if (other is CookingTool cookingTool)
        {
            foreach (var productInTool in cookingTool.GetProducts())
            {
                AddProductToPlate(productInTool);
            }
            cookingTool.EmptyTool();
            return;
        }

        if (other is Product product)
        {
            //AddProductToPlate(product);
        }
        
        Debug.Log("Product added to plate");
    }

    private void AddProductToPlate(Product product)
    {
        _products.Add(product);
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;
    }
}
