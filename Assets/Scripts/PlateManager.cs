using System;
using Tables;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlateManager: MonoBehaviour
    {
        [SerializeField] private PlateTable plateTable;
        [SerializeField] private SinkTable sinkTable;
        [SerializeField] private GameObject platePrefab;

        private void Start()
        {
            sinkTable.OnWashingCompleted += HandleWashingCompleted;
            AddPlates(3);
        }

        private void HandleWashingCompleted(object sender, EventArgs e)
        {
            AddPlates(1);
        }
        
        private void AddPlates(int count)
        {
            for (int i = 0; i < count; i++)
            {
                plateTable.AddCleanPlates(platePrefab);
            }
        }
    }
}