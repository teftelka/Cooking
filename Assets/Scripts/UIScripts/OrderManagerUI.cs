using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace UIScripts
{
    public class OrderManagerUI : MonoBehaviour
    {
        [SerializeField] private Transform orderTemplate;
        [SerializeField] private Transform container;
    
        private List<Transform> _activeOrderTemplates;
    
        private void Awake()
        {
            orderTemplate.gameObject.SetActive(false);
            _activeOrderTemplates = new List<Transform>();
        }
    
        private void Start()
        {
            OrderManager.Instance.OnNewRecipeSpawned += HandleNewRecipeSpawned;
            OrderManager.Instance.OnOrderComplete += HandleOrderComplete;
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            foreach (var order in _activeOrderTemplates)
            {
                if (e.recipeConplete == order.GetComponent<OrderTemplateUI>().GetRecipe())
                {
                    _activeOrderTemplates.Remove(order);
                    Destroy(order.gameObject);
                    break;
                }
            }
        }

        private void HandleNewRecipeSpawned(object sender, OrderManager.OnNewRecipeSpawnedEventArgs e)
        {
            RecipeSO newRecipe = e.newRecipe;
            UpdateVisual(newRecipe);
        }

        private void UpdateVisual(RecipeSO recipeToSpawn)
        {
            Transform newOrder = Instantiate(orderTemplate, container);
            newOrder.gameObject.SetActive(true);
            newOrder.GetComponent<OrderTemplateUI>().SetRecipe(recipeToSpawn);
        
            _activeOrderTemplates.Add(newOrder);
        
        }
    }
}
