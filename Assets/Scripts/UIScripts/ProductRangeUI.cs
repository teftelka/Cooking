using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class ProductRangeUI : MonoBehaviour
    {
        [SerializeField] private Product product;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private ProductRangeColorsSO rangeColorsSO;
        
        private void Awake()
        {
            SetActive(false);
        }
        
        public void UpdateRange(int range)
        {
            if (range <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            SetActive(true);
            rangeText.text = range.ToString();
            var a = rangeColorsSO._colors[range].color;
            backgroundImage.color = a;
        }
        
        public void SetActive(bool isActive)
        {
            rangeText.gameObject.SetActive(isActive);
            backgroundImage.gameObject.SetActive(isActive);
        }
    }
}
