using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class GardenPlateUI: MonoBehaviour
    {
        [SerializeField] private Image productIcon;
        [SerializeField] private TextMeshProUGUI productName;
        [SerializeField] private TextMeshProUGUI productAmount;

        private ProductSO currentSO;

        
        private void Start()
        {
            ResourceManager.Instance.OnResourceChanged += OnOnResourceChanged;
        }

        private void OnOnResourceChanged(object sender, ResourceManager.OnResourceChangedEventArgs e)
        {
            if (e.productSO == currentSO)
            {
                UpdateProductAmount();
            }
        }
        
        public void SetProduct(ProductSO productSo)
        {
            currentSO = productSo;
            productIcon.sprite = productSo.icon;
            productName.text = productSo.productName;
            UpdateProductAmount();
        }

        private void UpdateProductAmount()
        {
            productAmount.text = ResourceManager.Instance.GetAmount(currentSO).ToString();
        }
    }
}