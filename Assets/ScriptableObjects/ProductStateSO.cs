using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Product State")]
public class ProductStateSO : ScriptableObject
{
    public ProductState state;
    public Sprite sprite;
}