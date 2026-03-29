using UnityEngine;

public abstract class BaseObject: MonoBehaviour
{
    [SerializeField] protected BaseTable _originalTable;
    [SerializeField] protected ProductStateSO currentState;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected GameObject image;
    protected ProductState productState;

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
    
    public ProductState GetProductState()
    {
        return productState;
    }

    protected virtual void ApplyState(ProductStateSO state)
    {
        currentState = state;
        productState = currentState.state;
        spriteRenderer.sprite = state.sprite;
        spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }
    
    public virtual void Cut()
    {
        if (productState != ProductState.Raw) return;
        ApplyState(currentState.nextStateAsset);
    }
    
    public virtual bool CanMergeWith(BaseObject other) => false;
    public virtual bool CanMergeWithSelf(BaseObject other) => false;
    public virtual void Merge(BaseObject other) { }
}