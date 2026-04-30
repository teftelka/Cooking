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
        
        private void Start()
        {
            SetActive(false);
        }
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void UpdateRange(int range)
        {
            if (range <= 0f)
            {
                gameObject.SetActive(false);
                return;
            }
            
            SetActive(true);
            rangeText.text = range.ToString();
            var a = rangeColorsSO._colors[range].color;
            backgroundImage.color = a;
        }
    }
}
