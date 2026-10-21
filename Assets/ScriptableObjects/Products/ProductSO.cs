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

    [Header("Allowed actions from each state")]
    public List<ProductStateTransitionRule> rules;

    [Header("Base: holds other products (pizza dough, nori, bun)")]
    public ProductBaseConfig baseConfig = new ProductBaseConfig();

    public bool IsBase => baseConfig != null && baseConfig.isBase;

    public ProductStateTransitionRule FindRule(ProductStateSO current, ProductAction action)
    {
        if (rules == null) return null;

        foreach (var rule in rules)
        {
            if (rule.fromState == current && rule.action == action)
                return rule;
        }
        return null;
    }

    public bool TryGetNextState(ProductStateSO current, ProductAction action, out ProductStateSO next)
    {
        var rule = FindRule(current, action);
        next = rule?.toState;
        return rule != null;
    }

    public bool CanApply(ProductStateSO current, ProductAction action)
    {
        return FindRule(current, action) != null;
    }
}

[System.Serializable]
public class ProductBaseConfig
{
    [Tooltip("This product can hold other products as its ingredients")]
    public bool isBase;

    [Tooltip("The base accepts ingredients only while it is in this state")]
    public ProductStateSO acceptsInState;

    [Min(1)]
    public int maxContents = 4;

    [Tooltip("An ingredient is accepted only if the result can still become one of these recipes")]
    public RecipeListSO recipes;
}
