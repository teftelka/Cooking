using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate: BaseObject
{
    [SerializeField] private string type;
    private void GetDirty()
    {
        
    }

    private void GetClean()
    {
        
    }
    
    public void OnClick()
    {
        
    }

    public override string GetObjectType()
    {
        return type;
    }
}
