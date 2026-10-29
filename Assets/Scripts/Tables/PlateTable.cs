using System;
using System.Collections.Generic;
using Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace Tables
{
    public class PlateTable: BaseTable
    {
        private List<Plate> platesOnTable = new List<Plate>();

        public event EventHandler<OnPlateCountChangedEventArgs> OnPlateCountChanged;
        public class OnPlateCountChangedEventArgs {
            public int plateCount; 
        }
        
        public override void Interact(IHand hand)
        {
            if (hand.HasObject) return;
            if (platesOnTable.Count == 0) return;
            HandleObjectGive(hand);
        }

        private void HandleObjectGive(IHand hand)
        {
            platesOnTable.Remove(_objectOnTable as Plate);
            hand.TryTake(GiveObject());
            OnPlateCountChanged?.Invoke(this, new OnPlateCountChangedEventArgs { plateCount = platesOnTable.Count });
        }
        
        public void AddCleanPlates(GameObject platePrefab)
        {
            var newPlate = Instantiate(platePrefab, spawnPosition.transform.position, quaternion.identity).GetComponent<Plate>();
            SetObjectOnTable(newPlate);
        }
        
        public override void SetObjectOnTable(BaseObject _object)
        {
            _hasObject = true;
            _objectOnTable = _object;
            _object.transform.position = spawnPosition.transform.position;
            _object.RememberOrigin(this);
            platesOnTable.Add(_object as Plate);
            OnPlateCountChanged?.Invoke(this, new OnPlateCountChangedEventArgs { plateCount = platesOnTable.Count });
        }
        
        // Plates are stacked, so a plate can be returned even when others are already here
        public override bool TryReturnObject(BaseObject obj)
        {
            if (obj is not Plate) return false;
            SetObjectOnTable(obj);
            return true;
        }

        protected override BaseObject GiveObject()
        {
            BaseObject objectOnTable = _objectOnTable;
            if (platesOnTable.Count == 0)
            {
                _hasObject = false;
                _objectOnTable = null;
            }
            else
            {
                _objectOnTable = platesOnTable[^1];
            }

            return objectOnTable;
        }
    }
}
