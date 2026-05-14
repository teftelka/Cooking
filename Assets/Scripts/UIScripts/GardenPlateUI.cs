using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class GardenPlateUI: MonoBehaviour
    {
        [SerializeField] private Image productIcon;
        [SerializeField] private TextMeshProUGUI productName;
        
        public void SetProduct(ProductSO productSo)
        {
            productIcon.sprite = productSo.icon;
            productName.text = productSo.productName;
        }
    }
}