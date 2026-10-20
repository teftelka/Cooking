using ScriptableObjects;
using Tables;
using UnityEngine;

public class CookingStation : BaseTable
{
    [SerializeField]
    private CookingStationTypeSO stationType;
    
    public override bool CanPlace(BaseObject obj) =>
        obj is CookingTool tool && tool.CanWorkWith(stationType);

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
