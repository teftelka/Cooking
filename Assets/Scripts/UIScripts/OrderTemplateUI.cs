using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class OrderTemplateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderNameText;
        [SerializeField] private Transform iconsContainer;
        [SerializeField] private Transform iconTemplate;
        [SerializeField] private ProductRangeColorsSO rangeColorsSO;
    
        private Dish _order;
        
        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }
    
        public void SetOrder(Dish order)
        {
            _order = order;
            orderNameText.text = order.Name;

            foreach (var ingredient in _order.ingredients)
            {
                Transform newIcon = Instantiate(iconTemplate, iconsContainer);
                newIcon.gameObject.SetActive(true);
                newIcon.GetComponent<Image>().sprite = ingredient.productSO.icon;
                if (ingredient.productSO.isMergable && ingredient.productLevel > 0)
                {
                    var rarityImage = newIcon.GetChild(0);
                    rarityImage.gameObject.SetActive(true);
                    rarityImage.GetComponent<Image>().color = rangeColorsSO._colors[ingredient.productLevel].color;
                }
            }
        }
    
        public Dish GetOrder()
        {
            return _order;
        }
    }
}
