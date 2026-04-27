using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class OrderTemplateUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderNameText;
        [SerializeField] private Transform iconsContainer;
        [SerializeField] private Transform iconTemplate;
    
        private RecipeSO _recipe;
    
        public void SetRecipe(RecipeSO recipe)
        {
            _recipe = recipe;
            orderNameText.text = recipe.name;

            foreach (var ingredient in _recipe.ingredients)
            {
            }
        }
    
        public RecipeSO GetRecipe()
        {
            return _recipe;
        }
    }
}
