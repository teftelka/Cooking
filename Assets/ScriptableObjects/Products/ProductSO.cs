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

    [Tooltip("In this state the base turns into the dish of its recipe and shows the recipe's result prefab (e.g. baked dough). Empty = never")]
    public ProductStateSO dishState;

    [Tooltip("Pictures of the base with ingredients on it, while it accepts ingredients. " +
             "The order in which ingredients were added does not matter. If there is no exact match, " +
             "the entry with the most matching ingredients is used; if none, the state sprite")]
    public List<BaseContentsSprite> spritesByContents = new List<BaseContentsSprite>();

    // Best picture for these ingredients, or null to use the state sprite
    public Sprite FindSpriteFor(IReadOnlyList<IngredientData> contents)
    {
        if (spritesByContents == null || contents == null || contents.Count == 0) return null;

        BaseContentsSprite best = null;
        foreach (var entry in spritesByContents)
        {
            if (entry == null || entry.sprite == null || entry.ingredients == null || entry.ingredients.Count == 0)
                continue;
            if (!entry.IsSubsetOf(contents)) continue;

            if (entry.ingredients.Count == contents.Count)
                return entry.sprite;                      // exact match

            if (best == null || entry.ingredients.Count > best.ingredients.Count)
                best = entry;                             // largest subset so far (first wins on a tie)
        }
        return best?.sprite;
    }
}

[System.Serializable]
public class BaseContentsSprite
{
    public List<BaseIngredientKey> ingredients = new List<BaseIngredientKey>();
    public Sprite sprite;

    // Every listed ingredient has its own matching ingredient on the base (levels ignored)
    public bool IsSubsetOf(IReadOnlyList<IngredientData> contents)
    {
        if (ingredients.Count > contents.Count) return false;

        var used = new bool[contents.Count];

        // Keys with a specific state first, so "any state" keys don't take their ingredients
        foreach (var pass in new[] { true, false })
        {
            foreach (var key in ingredients)
            {
                if (key == null || key.product == null) return false;
                bool specific = key.state != null;
                if (specific != pass) continue;

                int found = -1;
                for (int i = 0; i < contents.Count; i++)
                {
                    if (used[i] || contents[i].product != key.product) continue;
                    if (specific && contents[i].state != key.state) continue;
                    found = i;
                    break;
                }
                if (found == -1) return false;
                used[found] = true;
            }
        }
        return true;
    }
}

[System.Serializable]
public class BaseIngredientKey
{
    public ProductSO product;

    [Tooltip("Empty = any state")]
    public ProductStateSO state;
}
