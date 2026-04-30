using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductSO", menuName = "Scriptable Objects/ProductSO")]
public class ProductSO : ScriptableObject
{
    public ProductType type;

    public Sprite icon;
    public GameObject prefab;
    public int price;

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
