using UnityEngine;

public abstract class BaseObject: MonoBehaviour
{
    [SerializeField] private BaseTable _originalTable;

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
    
    public virtual bool CanMergeWith(BaseObject other) => false;
    public virtual bool CanMergeWithSelf(BaseObject other) => false;
    public virtual void Merge(BaseObject other) { }
}