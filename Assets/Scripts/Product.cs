using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Product : BaseObject, IClickable
{
    [SerializeField] private string type;
    
    public void OnClick()
    {
        
    }

    public override string GetObjectType()
    {
        return type;
    }
}
