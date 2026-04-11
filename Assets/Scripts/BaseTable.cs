using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public  class BaseTable: MonoBehaviour
{
    [SerializeField] protected GameObject spawnPosition;
    
    [SerializeField] protected  bool _hasObject;
    [SerializeField] protected  BaseObject _objectOnTable;


    public virtual void SetObjectOnTable(BaseObject _object)
    {
        _hasObject = true;
        _objectOnTable = _object;
        _object.transform.position = spawnPosition.transform.position;
        _object.RememberOrigin(this);
    }

    protected virtual BaseObject GiveObject()
    {
        BaseObject objectOnTable = _objectOnTable;
        _hasObject = false;
        _objectOnTable = null;
        return objectOnTable;
    }
}
