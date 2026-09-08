using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductSO", menuName = "Scriptable Objects/ProductSO")]
public class ProductSO : ScriptableObject
{
    public string productName;
    public Sprite icon;
    public GameObject prefab;
    public int price;
    public bool isMergable;
    public ProductStateSO startState;

    [Header("Ingredient base (zero capacity means an ordinary product)")]
    [Min(0)] public int ingredientCapacity;
    public List<ProductState> ingredientReceivingStates = new() { ProductState.Raw };
    [Tooltip("Allowed ingredient types and states. Product level is ignored. Empty list accepts nothing.")]
    public List<RecipeItem> allowedIngredients = new();

    public bool CanReceiveIngredient(ProductState state, RecipeItem ingredient)
    {
        return ingredientCapacity > 0 && ingredientReceivingStates.Contains(state) &&
               allowedIngredients.Exists(item => item.productSO == ingredient.productSO &&
                                                 item.productState == ingredient.productState);
    }

    [Header("Allowed actions from each state")]
    public List<ProductStateTransitionRule> rules;

    public bool TryGetNextState(ProductStateSO current, ProductAction action, out ProductStateSO next)
    {
        foreach (var rule in rules)
        {
            if (rule.fromState == current && rule.action == action)
            {
                next = rule.toState;
                return true;
            }
        }

        next = null;
        return false;
    }

    public bool CanApply(ProductStateSO current, ProductAction action)
    {
        return rules.Exists(r => r.fromState == current && r.action == action);
    }
}
