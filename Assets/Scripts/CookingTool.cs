using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

public class CookingTool : BaseObject, IProductContainer, IHasProgress
{
    public event EventHandler<ProgressChangedEventArgs> OnProgressChanged;

    // One args object reused every frame instead of a new one per event
    private readonly ProgressChangedEventArgs progressArgs = new ProgressChangedEventArgs();

    private void RaiseProgress(float progress)
    {
        progressArgs.progress = progress;
        OnProgressChanged?.Invoke(this, progressArgs);
    }

    public event EventHandler OnContentsChanged;
    
    [SerializeField] private ProductAction toolAction;
    [SerializeField] private List<CookingStationTypeSO> compatibleStations;
    
    [SerializeField] private List<Product> _products;
    [SerializeField] private int capacity = 3;
    [SerializeField] private float cookingTime = 5f;
    [SerializeField] private float burningTime = 4f;
    [SerializeField] private float extraTimePerItem = 2f;
    [SerializeField] private CookingProgressState _state;
    // The shared process of everything inside: time spent on heat in the current stage
    // and the length of that stage (it grows as products are added)
    [SerializeField] private float _elapsed;
    [SerializeField] private float _required;
    [SerializeField] private bool _isOnHeat;
    
    private enum CookingProgressState
    {
        Idle,
        Cooking,
        Burning,
        Burned
    }

    private void Start()
    {
        _state = CookingProgressState.Idle;
    }
    
    private void Update()
    {
        if (!_isOnHeat) return;
        if (_products.Count == 0) return;
        if (_state != CookingProgressState.Cooking && _state != CookingProgressState.Burning) return;

        _elapsed += Time.deltaTime;
        RaiseProgress(_elapsed / _required);

        if (_elapsed < _required) return;

        switch (_state)
        {
            case CookingProgressState.Cooking:
                CookRecipe();
                StartStage(CookingProgressState.Burning, 0f, burningTime);
                Debug.Log("Food cooked");
                break;
            case CookingProgressState.Burning:
                BurnRecipe();
                _state = CookingProgressState.Burned;
                RaiseProgress(1f);   // finished: the bar hides
                Debug.Log("Food burned!");
                break;
        }
    }

    private void StartStage(CookingProgressState state, float elapsed, float required)
    {
        _state = state;
        _elapsed = elapsed;
        _required = Mathf.Max(required, 0.01f);   // never divide by zero
    }

    // Contents leave only when cooked (or burned), not in the middle of cooking
    public bool CanReleaseContents => _state != CookingProgressState.Cooking;
    
    public void SetHeat(bool hasHeat)
    {
        _isOnHeat = hasHeat;
    }
    
    private void CookRecipe()
    {
        foreach (var product in _products)
        {
            product.ApplyAction(toolAction);
        }
    }

    private void BurnRecipe()
    {
        foreach (var product in _products)
        {
            if (product.CanApplyAction(ProductAction.Burn))
            {
                product.ApplyAction(ProductAction.Burn);
            }
        }
    }
    
    public bool CanWorkWith(CookingStationTypeSO stationType)
    {
        return compatibleStations.Contains(stationType);
    }

    public IReadOnlyList<Product> GetProducts()
    {
        return _products;
    }

    public void EmptyContainer()
    {
        _state = CookingProgressState.Idle;
        _products.Clear();
        RaiseProgress(0f);   // nothing inside: the bar hides
        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DestroyContents()
    {
        foreach (var product in _products)
        {
           Destroy(product.gameObject); 
        }

        EmptyContainer();
    }
    
    // A product from hand, or the single base lying on a plate (pizza from a plate into the oven).
    // Ordinary products never go from a plate into cookware.
    private static Product GetIncomingProduct(BaseObject other)
    {
        switch (other)
        {
            case Product product:
                return product;
            case Plate plate:
                return plate.GetSingleBase();
            default:
                return null;
        }
    }

    public override bool CanAccept(BaseObject other)
    {
        var product = GetIncomingProduct(other);
        if (product == null) return false;
        if (_products.Count >= capacity) return false;
        if (_state == CookingProgressState.Burned) return false;
        
        return product.CanApplyAction(toolAction);
    }

    public override void Accept(BaseObject other)
    {
        Product product = other is Plate plate ? plate.ReleaseSingleBase() : other as Product;
        if (product == null) return;

        _products.Add(product);

        switch (_state)
        {
            case CookingProgressState.Idle:
                StartStage(CookingProgressState.Cooking, 0f, cookingTime);
                break;
            case CookingProgressState.Cooking:
                // The dish takes a bit longer: the bar rolls back a little
                _required += extraTimePerItem;
                break;
            case CookingProgressState.Burning:
                // Burning is interrupted: back to cooking, ready again in extraTimePerItem
                StartStage(CookingProgressState.Cooking, Mathf.Max(cookingTime - extraTimePerItem, 0f), cookingTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        product.SetToParent(transform);
        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }
}
