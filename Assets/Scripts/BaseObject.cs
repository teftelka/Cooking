using UnityEngine;

public abstract class BaseObject: MonoBehaviour
{
    public virtual bool CanMergeWith(BaseObject other) => false;
    public virtual void Merge(BaseObject other) { }
}