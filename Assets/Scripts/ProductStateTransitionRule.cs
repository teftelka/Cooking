[System.Serializable]
public class ProductStateTransitionRule
{
    public ProductStateSO fromState;
    public ProductAction action;
    public ProductStateSO toState;

    [UnityEngine.Tooltip("Base products only: the action is allowed only if, after it, the base with its ingredients exactly forms a recipe (e.g. bake only a complete pizza)")]
    public bool requiresRecipe;
}