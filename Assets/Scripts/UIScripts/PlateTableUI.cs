using System;
using Tables;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class PlateTableUI: MonoBehaviour
    {
        [SerializeField] private PlateTable plateTable;
        [SerializeField] private TextMeshProUGUI plateCountText;
        
        private void Start()
        {
            plateTable.OnPlateCountChanged += OnPlateCountChanged;
        }

        private void OnPlateCountChanged(object sender, PlateTable.OnPlateCountChangedEventArgs e)
        {
            UpdateVisual(e.plateCount);
        }


        private void UpdateVisual(int plateCount)
        {
            plateCountText.text = plateCount.ToString();
        }
    }
}