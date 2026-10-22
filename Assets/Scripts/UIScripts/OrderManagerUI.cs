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
                if (e.completedOrder == order.GetComponent<OrderTemplateUI>().GetOrder())
                {
                    _activeOrderTemplates.Remove(order);
                    Destroy(order.gameObject);
                    break;
                }
            }
        }

        private void HandleNewRecipeSpawned(object sender, OrderManager.OnNewRecipeSpawnedEventArgs e)
        {
            UpdateVisual(e.newOrder);
        }

        private void UpdateVisual(Dish orderToShow)
        {
            Transform newOrder = Instantiate(orderTemplate, container);
            newOrder.gameObject.SetActive(true);
            newOrder.GetComponent<OrderTemplateUI>().SetOrder(orderToShow);
        
            _activeOrderTemplates.Add(newOrder);
        
        }
    }
}
