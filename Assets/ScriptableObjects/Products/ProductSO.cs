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

#if UNITY_EDITOR
    // ---- Editor helpers for the base's "Sprites By Contents" table (three-dot menu of this asset in the Inspector) ----

    private struct FillingKey
    {
        public ProductSO product;
        public ProductState state;
    }

    [ContextMenu("Base: Check contents sprites")]
    private void CheckContentsSprites()
    {
        if (!TryGetReachableCombos(out var combos)) return;

        var entries = baseConfig.spritesByContents ?? new List<BaseContentsSprite>();
        int problems = 0;

        foreach (var combo in combos)
        {
            if (entries.Exists(e => e != null && e.sprite != null && CoversExactly(e, combo))) continue;
            problems++;
            Debug.LogWarning($"{name}: no picture for: {Describe(combo)}", this);
        }

        var recipeFillings = GetRecipeFillings();
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry == null || entry.ingredients == null || entry.ingredients.Count == 0)
            {
                problems++;
                Debug.LogWarning($"{name}: entry #{i} has no ingredients", this);
                continue;
            }
            if (entry.sprite == null)
            {
                problems++;
                Debug.LogWarning($"{name}: entry #{i} ({DescribeEntry(entry)}) has no sprite", this);
            }
            if (!recipeFillings.Exists(fillings => FitsInto(entry, fillings)))
            {
                problems++;
                Debug.LogWarning($"{name}: entry #{i} ({DescribeEntry(entry)}) can never be used: no recipe with this base has these ingredients", this);
            }
            for (int j = 0; j < i; j++)
            {
                if (entries[j] != null && entries[j].ingredients != null && EntryKey(entries[j]) == EntryKey(entry))
                {
                    problems++;
                    Debug.LogWarning($"{name}: entries #{j} and #{i} have the same ingredients", this);
                }
            }
        }

        if (problems == 0)
            Debug.Log($"{name}: OK, all {combos.Count} possible contents have a picture", this);
    }

    [ContextMenu("Base: Add missing contents sprites")]
    private void AddMissingContentsSprites()
    {
        if (!TryGetReachableCombos(out var combos)) return;

        UnityEditor.Undo.RecordObject(this, "Add missing contents sprites");
        if (baseConfig.spritesByContents == null)
            baseConfig.spritesByContents = new List<BaseContentsSprite>();

        int added = 0;
        foreach (var combo in combos)
        {
            if (baseConfig.spritesByContents.Exists(e => e != null && CoversExactly(e, combo))) continue;

            var entry = new BaseContentsSprite();
            foreach (var key in combo)
                entry.ingredients.Add(new BaseIngredientKey { product = key.product, state = FindStateAsset(key.product, key.state) });

            baseConfig.spritesByContents.Add(entry);
            added++;
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"{name}: added {added} entries without a picture, fill in their sprites", this);
    }

    // Ingredients (without the base itself) of every recipe that uses this base
    private List<List<FillingKey>> GetRecipeFillings()
    {
        var result = new List<List<FillingKey>>();
        var list = baseConfig != null ? baseConfig.recipes : null;
        if (list == null || list.allRecipes == null) return result;

        foreach (var recipe in list.allRecipes)
        {
            if (recipe == null || recipe.ingredients == null) continue;

            int baseIndex = recipe.ingredients.FindIndex(r => r.productSO == this);
            if (baseIndex == -1) continue;

            var fillings = new List<FillingKey>();
            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (i == baseIndex || recipe.ingredients[i].productSO == null) continue;
                fillings.Add(new FillingKey { product = recipe.ingredients[i].productSO, state = recipe.ingredients[i].productState });
            }
            result.Add(fillings);
        }
        return result;
    }

    // Every combination of ingredients the base can really have: all non-empty parts of each recipe
    private bool TryGetReachableCombos(out List<List<FillingKey>> combos)
    {
        combos = new List<List<FillingKey>>();
        if (!IsBase)
        {
            Debug.LogWarning($"{name}: not a base (Is Base is off)", this);
            return false;
        }
        if (baseConfig.recipes == null)
        {
            Debug.LogWarning($"{name}: Recipes is empty", this);
            return false;
        }

        var seen = new HashSet<string>();
        foreach (var fillings in GetRecipeFillings())
        {
            int n = fillings.Count;
            if (n > 16)
            {
                Debug.LogWarning($"{name}: a recipe has too many ingredients to list all combinations", this);
                continue;
            }

            for (int mask = 1; mask < (1 << n); mask++)
            {
                var combo = new List<FillingKey>();
                for (int i = 0; i < n; i++)
                {
                    if ((mask & (1 << i)) != 0) combo.Add(fillings[i]);
                }
                if (seen.Add(ComboKey(combo))) combos.Add(combo);
            }
        }

        if (combos.Count == 0)
            Debug.LogWarning($"{name}: no recipe in {baseConfig.recipes.name} uses this base with ingredients", this);
        return combos.Count > 0;
    }

    // The entry lists exactly these ingredients (an empty state in the entry matches any state)
    private static bool CoversExactly(BaseContentsSprite entry, List<FillingKey> combo)
    {
        return entry.ingredients != null && entry.ingredients.Count == combo.Count && FitsInto(entry, combo);
    }

    // Every ingredient of the entry has its own matching ingredient in the set
    private static bool FitsInto(BaseContentsSprite entry, List<FillingKey> set)
    {
        if (entry.ingredients == null || entry.ingredients.Count > set.Count) return false;

        var used = new bool[set.Count];
        foreach (var specificPass in new[] { true, false })
        {
            foreach (var key in entry.ingredients)
            {
                if (key == null || key.product == null) return false;
                bool specific = key.state != null;
                if (specific != specificPass) continue;

                int found = -1;
                for (int i = 0; i < set.Count; i++)
                {
                    if (used[i] || set[i].product != key.product) continue;
                    if (specific && set[i].state != key.state.state) continue;
                    found = i;
                    break;
                }
                if (found == -1) return false;
                used[found] = true;
            }
        }
        return true;
    }

    // A recipe stores a state as an enum value; the table needs the state asset of that product
    private static ProductStateSO FindStateAsset(ProductSO product, ProductState state)
    {
        if (product.startState != null && product.startState.state == state) return product.startState;
        if (product.rules != null)
        {
            foreach (var rule in product.rules)
            {
                if (rule.toState != null && rule.toState.state == state) return rule.toState;
                if (rule.fromState != null && rule.fromState.state == state) return rule.fromState;
            }
        }
        return null; // not found: the entry will accept any state
    }

    private static string ComboKey(List<FillingKey> combo)
    {
        var parts = combo.ConvertAll(k => k.product.GetInstanceID() + ":" + (int)k.state);
        parts.Sort();
        return string.Join("|", parts);
    }

    private static string EntryKey(BaseContentsSprite entry)
    {
        var parts = entry.ingredients.ConvertAll(k =>
            k == null || k.product == null ? "?" : k.product.GetInstanceID() + ":" + (k.state != null ? k.state.GetInstanceID().ToString() : "*"));
        parts.Sort();
        return string.Join("|", parts);
    }

    private static string Describe(List<FillingKey> combo)
    {
        return string.Join(" + ", combo.ConvertAll(k => $"{k.product.name} ({k.state})"));
    }

    private static string DescribeEntry(BaseContentsSprite entry)
    {
        return string.Join(" + ", entry.ingredients.ConvertAll(k =>
            k == null || k.product == null ? "?" : $"{k.product.name} ({(k.state != null ? k.state.name : "any state")})"));
    }
#endif
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
