[System.Serializable]
public class ProductStateTransitionRule
{
    public ProductStateSO fromState;
    public ProductAction action;
    public ProductStateSO toState;
}