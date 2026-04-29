using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Product State")]
public class ProductStateSO : ScriptableObject
{
    public ProductState state;
    public Sprite sprite;

    /*public List<ProductTransition> transitions;

    public bool TryGetTransition(ProductAction action, out ProductStateSO next)
    {
        foreach (var transition in transitions)
        {
            if (transition.action == action)
            {
                next = transition.targetState;
                return true;
            }
        }

        next = null;
        return false;
    }
    
    public bool HasTransition(ProductAction action)
    {
        return transitions.Any(t => t.action == action);
    }*/
}