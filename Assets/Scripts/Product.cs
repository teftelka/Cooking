using UnityEngine;

public class Product : BaseObject, IClickable
{
    [SerializeField] private ProductType type;
    [SerializeField] private int range;
    [SerializeField] private ProductState productState;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] protected ProductStateSO currentState;
    [SerializeField] private Sprite defaultSprite;
    

    public void Start()
    {
        ApplyState(currentState);
        defaultSprite = spriteRenderer.sprite;
        range = 0;
    }

    public void OnClick()
    {
        
    }
    
    /*private ProductState GetProductState()
    {
        return productState;
    }
    
    private ProductType GetObjectType()
    {
        return type;
    }*/
    
    private void ApplyState(ProductStateSO state)
    {
        currentState = state;
        productState = currentState.state;
        spriteRenderer.sprite = state.sprite;
        spriteRenderer.transform.localScale = new Vector3(0.35f, 0.35f, 1f);
    }
    
    public bool CanApplyAction(ProductAction action)
    {
        return currentState.HasTransition(action);
    }
    
    public bool ApplyAction(ProductAction action)
    {
        if (!currentState.TryGetTransition(action, out var nextState))
            return false;

        ApplyState(nextState);
        return true;
    }
    
    public RecipeItem GetRecipeKey()
    {
        return new RecipeItem
        {
            productType = type,
            productState = productState,
            productLevel = range,
            icon = spriteRenderer.sprite
        };
    }
    
    public void SetToParent(Transform parent)
    {
        //DisableImage();
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public void DisableImage()
    {
        spriteRenderer.gameObject.SetActive(false);
    }
    
    public override bool CanCombineWith(BaseObject other)
    {
        if (other is not Product otherProduct) return false;
        if (type != otherProduct.type) return false;
        if (otherProduct.productState != ProductState.Raw) return false;
        if (productState != ProductState.Raw) return false;
        
        return true;
    }

    public override void CombineWith(BaseObject other)
    {
        Product otherProduct = (Product)other;
        range++;
        Destroy(otherProduct.gameObject);

        Debug.Log("Products combined -> upgraded");
    }
    
    public Sprite GetDefaultSprite()
    {
        return defaultSprite;
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
