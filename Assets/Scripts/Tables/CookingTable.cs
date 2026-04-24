using System;
using DefaultNamespace;
using UnityEngine;

namespace Tables
{
    public class CookingTable: BaseTable, IClickable
    {
        public void OnClick()
        {
            var playerProduct = PlayerTest.Instance.GetProduct();

            if (playerProduct)
            {
                if (_hasObject)
                {
                    HandleCollision(playerProduct);
                    return;
                }
                TakeObject(playerProduct);
                return;
            }
            if (_hasObject)
            {
                HandleObjectGive();
            }
        }

        private void HandleObjectGive()
        {
            if (_objectOnTable is CookingTool tool)
            {
                tool.SetHeat(false);
            }
                
            PlayerTest.Instance.HandleObjectTake(GiveObject());
        }

        private void HandleCollision(BaseObject objectInHand)
        {
            if (_objectOnTable.CanAccept(objectInHand))
            {
                _objectOnTable.Accept(objectInHand);
                PlayerTest.Instance.HandleObjectGive();
            }
        }

        private void TakeObject(BaseObject objectInHand)
        {
            if (objectInHand is CookingTool cookingTool)
            {
                PlayerTest.Instance.HandleObjectGive();
                SetObjectOnTable(cookingTool);
            }
        }

        public override void SetObjectOnTable(BaseObject _object)
        {
            _hasObject = true;
            _objectOnTable = _object;
            _object.transform.position = spawnPosition.transform.position;
            _object.RememberOrigin(this);
            
            if (_object is CookingTool tool)
            {
                tool.SetHeat(true);
            }
        }
    }
}