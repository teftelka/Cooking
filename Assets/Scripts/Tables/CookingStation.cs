using ScriptableObjects;
using Tables;
using UnityEngine;

public class CookingStation : BaseTable
{
    [SerializeField]
    private CookingStationTypeSO stationType;

    [Tooltip("A tool that always stays on this station, e.g. the baking sheet inside an oven. " +
             "Must be a child object of the station (not a prefab asset). Leave empty for a normal stove.")]
    [SerializeField]
    private CookingTool fixedTool;

    private bool HasFixedTool => fixedTool != null;

    private void Start()
    {
        if (!HasFixedTool) return;

        if (!fixedTool.gameObject.scene.IsValid())
        {
            Debug.LogError($"{name}: the fixed tool must be a child object of the station, not a prefab asset", this);
            fixedTool = null;
            return;
        }

        SetObjectOnTable(fixedTool);   // OnPlaced turns the heat on, and it stays on
    }

    public override bool CanPlace(BaseObject obj) =>
        !HasFixedTool && obj is CookingTool tool && tool.CanWorkWith(stationType);

    // A fixed tool never leaves the station (no taking, no swapping)
    public override bool CanTake() => !HasFixedTool;

    // With a fixed tool, an empty hand takes what is inside it (e.g. the pizza), not the tool itself.
    // Only a single product can be taken by hand; with more, use a plate.
    public override BaseObject TakeForEmptyHand()
    {
        if (!HasFixedTool) return base.TakeForEmptyHand();

        // Only when cooked (or burned): nothing is taken out while it is still cooking
        if (!fixedTool.CanReleaseContents) return null;

        var products = fixedTool.GetProducts();
        if (products.Count != 1) return null;

        var product = products[0];
        fixedTool.EmptyContainer();
        product.DetachFromContainer();
        return product;
    }

    protected override void OnPlaced(BaseObject obj)
    {
        if (obj is CookingTool tool)
            tool.SetHeat(true);
    }

    protected override void OnTaken(BaseObject obj)
    {
        if (obj is CookingTool tool)
            tool.SetHeat(false);
    }
}
