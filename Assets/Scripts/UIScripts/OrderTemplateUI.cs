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
    
        private RecipeSO _recipe;
        
        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);
        }
    
        public void SetRecipe(RecipeSO recipe)
        {
            _recipe = recipe;
            orderNameText.text = recipe.recipeName;

            foreach (var ingredient in _recipe.ingredients)
            {
                Transform newIcon = Instantiate(iconTemplate, iconsContainer);
                newIcon.gameObject.SetActive(true);
                newIcon.GetComponent<Image>().sprite = ingredient.productSO.icon;
                var rarityImage = newIcon.GetChild(0);
                rarityImage.GetComponent<Image>().color = rangeColorsSO._colors[ingredient.productLevel].color;
            }
        }
    
        public RecipeSO GetRecipe()
        {
            return _recipe;
        }
    }
}
