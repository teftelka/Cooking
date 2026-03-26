using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class BaseTable: MonoBehaviour
{
    [SerializeField] protected GameObject spawnPosition;
    
    [SerializeField] protected  bool _hasObject;
    [SerializeField] protected  BaseObject _product;


    public virtual void SetObjectOnTable(BaseObject product)
    {
        _hasObject = true;
        _product = product;
        product.transform.position = spawnPosition.transform.position;
        product.RememberOrigin(this);
    }

    public virtual BaseObject TakeObject()
    {
        BaseObject objectOnTable = _product;
        _hasObject = false;
        _product = null;
        return objectOnTable;
    }
}
