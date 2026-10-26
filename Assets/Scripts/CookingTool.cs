using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects;
using UnityEngine;

public class CookingTool : BaseObject, IProductContainer
{
    public event EventHandler OnEmptyTool;
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float cookingProgress; 
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
    [SerializeField] private float _timer;
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

        _timer -= Time.deltaTime;
        OnProgressChanged?.Invoke(this, new OnProgressChangedEventArgs { cookingProgress = 1 - (_timer / ( _state == CookingProgressState.Cooking ? cookingTime : burningTime)) });
        
        if (_timer > 0f) return;

        switch (_state)
        {
            case CookingProgressState.Cooking:
                CookRecipe();
                _state = CookingProgressState.Burning;
                _timer = burningTime;
                Debug.Log("Food cooked");
                break;
            case CookingProgressState.Burning:
                BurnRecipe();
                _state = CookingProgressState.Burned;
                Debug.Log("Food burned!");
                break;
        }
    }
    
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
        OnEmptyTool?.Invoke(this, EventArgs.Empty);
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
                _state = CookingProgressState.Cooking;
                _timer = cookingTime;
                break;
            case CookingProgressState.Cooking:
                _timer += extraTimePerItem;
                break;
            case CookingProgressState.Burning:
                _state = CookingProgressState.Cooking;
                _timer = extraTimePerItem;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        product.SetToParent(transform);
        OnContentsChanged?.Invoke(this, EventArgs.Empty);
    }
}
