using Tables;
using UnityEngine;

public abstract class BaseObject: MonoBehaviour
{
    [SerializeField] protected BaseTable _originalTable;

    public void RememberOrigin(BaseTable table)
    {
        _originalTable = table;
    }

    // Returns false if there is no origin or the origin table refuses the object
    public bool ReturnToOrigin()
    {
        return _originalTable != null && _originalTable.TryReturnObject(this);
    }
    
    public BaseTable GetOrigin()
    {
        return _originalTable;
    }

    public virtual bool CanAccept(BaseObject other) => false;
    public virtual bool CanBeAcceptedBy(BaseObject other) => false;
    public virtual void Accept(BaseObject other) { }


    public virtual bool CanCombineWith(BaseObject other) => false;
    public virtual void CombineWith(BaseObject other) { }
}