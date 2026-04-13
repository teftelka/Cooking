using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Product State")]
public class ProductStateSO : ScriptableObject
{
    public ProductState state;
    public Sprite sprite;

    public List<ProductTransition> transitions;

    public bool TryGetTransition(ProductAction action, out ProductStateSO next)
    {
        foreach (var t in transitions)
        {
            if (t.action == action)
            {
                next = t.targetState;
                return true;
            }
        }

        next = null;
        return false;
    }
}