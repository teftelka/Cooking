using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class GardenPlateUI: MonoBehaviour
    {
        [SerializeField] private GardenPlate gardenPlate;
        [SerializeField] private Image productIcon;
        [SerializeField] private TextMeshProUGUI productName;
        [SerializeField] private TextMeshProUGUI productAmount;
        [SerializeField] private TextMeshProUGUI productExperience;
        [SerializeField] private TextMeshProUGUI productLevel;
        [SerializeField] private Image progressBarFill;

        private ProductSO currentSO;

        
        private void Start()
        {
            ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
            ProductExperienceManager.Instance.OnExperienceChanged += OnOnExperienceChanged;
            ProductExperienceManager.Instance.OnLevelChanged += InstanceOnLevelChanged;
            gardenPlate.OnProgressChanged += OnProgressChanged;
        }

        private void InstanceOnLevelChanged(object sender, ProductExperienceManager.OnLevelChangedEventArgs e)
        {
            if (e.productSO == currentSO)
            {
                UpdateProductAmount();
                productLevel.text = e.level.ToString();
            }
        }

        private void OnOnExperienceChanged(object sender, ProductExperienceManager.OnExperienceChangedEventArgs e)
        {
            if (e.productSO == currentSO)
            {
                UpdateProductExp(e.exp);
            }
        }

        private void OnProgressChanged(object sender, GardenPlate.OnProgressChangedEventArgs e)
        {
            ProgressChanged(e.spawningProgress);
        }

        private void OnResourceChanged(object sender, ResourceManager.OnResourceChangedEventArgs e)
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
            productExperience.text  = "0";
            UpdateProductAmount();
        }

        private void UpdateProductAmount()
        {
            int productRange = ProductExperienceManager.Instance.GetLevel(currentSO);
            productAmount.text = ResourceManager.Instance.GetAmount(currentSO, productRange).ToString();
        }
        
        private void UpdateProductExp(int exp)
        {
            productExperience.text  = exp.ToString();
        }
        
        private void ProgressChanged(float progress)
        {
            //SetVisibility(!(progressBarFill.fillAmount >= 0.99f));
            progressBarFill.fillAmount = progress;
        }
    }
}