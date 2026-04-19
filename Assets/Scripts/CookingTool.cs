using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;

public class CookingTool : BaseObject
{
    [SerializeField] private ProductAction toolAction;
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

        _timer -= Time.deltaTime;
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

    public List<Product> GetProducts()
    {
        return _products;
    }

    public void EmptyTool()
    {
        _state = CookingProgressState.Idle;
        _products.Clear();
    }

    public void DestroyAllProducts()
    {
        foreach (var product in _products)
        {
           Destroy(product.gameObject); 
        }

        EmptyTool();
    }
    
    public override bool CanAccept(BaseObject other)
    {
        if (other is not Product product) return false;
        if (_products.Count >= capacity) return false;
        if (_state == CookingProgressState.Burned) return false;
        
        return product.CanApplyAction(toolAction);
    }

    public override void Accept(BaseObject other)
    {
        Product product = (Product)other;
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
    }
}
