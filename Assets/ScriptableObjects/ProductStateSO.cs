using UnityEngine;

[CreateAssetMenu(fileName = "ProductStateSO", menuName = "Scriptable Objects/ProductStateSO")]
public class ProductStateSO : ScriptableObject
{
    public ProductState state;

    public Sprite sprite;
    public ProductStateSO nextStateAsset;

}
