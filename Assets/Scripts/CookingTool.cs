using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tables;
using UnityEngine;

public class CookingTool : BaseObject
{
    [SerializeField] private ProductAction toolAction;
    //[SerializeField] private bool hasObject;
    [SerializeField] private List<Product> _products;
    [SerializeField] private int capacity = 3;
    [SerializeField] private float cookingTime = 5f;
    [SerializeField] private float burningTime = 4f;
    private CookingProgressState _state;
    [SerializeField] private float _timer;
    [SerializeField] private bool _isOnHeat;
    
    private enum CookingProgressState
    {
        Idle,
        Cooking,
        Burning
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
                _state = CookingProgressState.Idle;
                Debug.Log("Food burned!");
                break;
        }
    }
    
    public void SetHeat(bool hasHeat)
    {
        _isOnHeat = hasHeat;
        
        if (_isOnHeat && CanCook() && _state == CookingProgressState.Idle)
        {
            _state = CookingProgressState.Cooking;
            _timer = cookingTime;
        }
    }
    
    private bool CanCook()
    {
        return _products.Any(product => product.CanApplyAction(toolAction));
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
    
    public override bool CanAccept(BaseObject other)
    {
        if (other is not Product product) return false;
        if (_products.Count >= capacity) return false;

        return product.CanApplyAction(toolAction);
    }

    public override void Accept(BaseObject other)
    {
        Product product = (Product)other;
        _products.Add(product);
        
        product.transform.SetParent(transform);
        product.transform.localPosition = Vector3.zero;
    }
}
