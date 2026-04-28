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
                newIcon.GetComponent<Image>().sprite = ingredient.productPrefab.GetComponent<Product>().GetDefaultSprite();
            }
        }
    
        public RecipeSO GetRecipe()
        {
            return _recipe;
        }
    }
}
