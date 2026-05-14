using System;
using System.Collections.Generic;
using DefaultNamespace;
using Interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace Tables
{
    public class PlateTable: BaseTable, IClickable
    {
        private List<Plate> platesOnTable = new List<Plate>();

        public EventHandler<OnPlateCountChangedEventArgs> OnPlateCountChanged;
        public class OnPlateCountChangedEventArgs {
            public int plateCount; 
        }
        
        public void OnClick()
        {
            if (PlayerTest.Instance.HasObject()) return;
            if (platesOnTable.Count == 0) return;
            HandleObjectGive();
        }

        private void HandleObjectGive()
        {
            platesOnTable.Remove(_objectOnTable as Plate);
            PlayerTest.Instance.HandleObjectTake(GiveObject());
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