using DefaultNamespace;
using Interfaces;
using ScriptableObjects;
using Tables;
using UnityEngine;

public class CookingStation : BaseTable, IClickable
{
    [SerializeField]
    private CookingStationTypeSO stationType;

    public void OnClick()
    {
        var playerObject = PlayerTest.Instance.GetProduct();

        if (playerObject)
        {
            if (_hasObject)
            {
                HandleCollision(playerObject);
                return;
            }

            TakeObject(playerObject);
            return;
        }

        if (_hasObject)
        {
            HandleObjectGive();
        }
    }

    private void TakeObject(BaseObject objectInHand)
    {
        if (!CanAcceptTool(objectInHand))
            return;

        PlayerTest.Instance.HandleObjectGive();
        SetObjectOnTable(objectInHand);
    }

    private bool CanAcceptTool(BaseObject obj)
    {
        return obj is CookingTool tool &&
               tool.CanWorkWith(stationType);
    }

    public override void SetObjectOnTable(BaseObject obj)
    {
        _hasObject = true;
        _objectOnTable = obj;

        obj.transform.position = spawnPosition.transform.position;
        obj.RememberOrigin(this);

        if (obj is CookingTool tool)
            tool.SetHeat(true);
    }

    private void HandleObjectGive()
    {
        if (_objectOnTable is CookingTool tool)
        {
            if (!tool.CanMove())
            {
                return;
            }
            tool.SetHeat(false);
        }
        
        PlayerTest.Instance.HandleObjectTake(GiveObject());
    }

    private void HandleCollision(BaseObject objectInHand)
    {
        if (_objectOnTable.CanAccept(objectInHand))
        {
            _objectOnTable.Accept(objectInHand);
            if (objectInHand is IProductContainer && objectInHand is not Product) return;
            PlayerTest.Instance.HandleObjectGive();
            return;
        }

        if (objectInHand.CanAccept(_objectOnTable))
        {
            objectInHand.Accept(_objectOnTable);
            if (_objectOnTable is IProductContainer && _objectOnTable is not Product) return;
            _objectOnTable = null;
            _hasObject = false;
        }
    }
}
