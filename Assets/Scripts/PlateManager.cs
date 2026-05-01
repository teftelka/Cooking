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
        }

        private void HandleWashingCompleted(object sender, EventArgs e)
        {
            plateTable.AddCleanPlates(platePrefab);
        }
    }
}