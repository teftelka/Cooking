using UnityEngine;

public abstract class BaseObject: MonoBehaviour
{
    [SerializeField] protected BaseTable _originalTable;
    [SerializeField] protected ProductStateSO currentState;

    public void RememberOrigin(BaseTable table)
    {
        _originalTable = table;
    }

    public void ReturnToOrigin()
    {
        _originalTable?.SetObjectOnTable(this);
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