using System;
using System.Collections.Generic;
using DefaultNamespace;
using Tables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class OrderManager: MonoBehaviour
    {
        public static OrderManager Instance { get; set; }
    
        [SerializeField] private RecipeListSO recipeList;
        [SerializeField] private List<Dish> _activeOrders;
        private float spawnRecipeTimer;
        private float spawnRecipeTimerMax = 3f;
        [SerializeField] private int maxActiveOrders = 2;
    
        public EventHandler<OnNewRecipeSpawnedEventArgs> OnNewRecipeSpawned;
        public class OnNewRecipeSpawnedEventArgs : EventArgs
        {
            public Dish newOrder;
        }
    
        public EventHandler<OnOrderCompleteEventArgs> OnOrderComplete;
        public class OnOrderCompleteEventArgs : EventArgs
        {
            public Dish completedOrder;
        }
    
        public event EventHandler<OnOrderRejectedEventArgs> OnOrderRejected;
        public class OnOrderRejectedEventArgs : EventArgs
        {
            public OrderRejectReason reason;
            public Dish dish;          // the finished dish on the plate, or null if there is none
            public OrderTable table;   // where it happened, to show the message there
        }

        private void Awake()
        {
            Instance = this;
            _activeOrders = new List<Dish>();
        }
    
        private void Update()
        {
            spawnRecipeTimer -= Time.deltaTime;
            if (spawnRecipeTimer <= 0f)
            {
                spawnRecipeTimer = spawnRecipeTimerMax;
                if (_activeOrders.Count < maxActiveOrders)
                {
                    SpawnNewOrder();
                }
            }
        }

        private void SpawnNewOrder()
        {
            var nextOrderRecipe = recipeList.allRecipes[UnityEngine.Random.Range(0, recipeList.allRecipes.Count)];
            var order = CreateOrder(nextOrderRecipe);
        
            OnNewRecipeSpawned?.Invoke(this, new OnNewRecipeSpawnedEventArgs { newOrder = order });
        
            _activeOrders.Add(order);
        }

        // An order is the recipe with a random required level for every mergeable ingredient
        private Dish CreateOrder(RecipeSO nextOrderRecipe)
        {
            var recipeItems = new List<RecipeItem>();

            foreach (var ingredient in nextOrderRecipe.ingredients)
            {
                var randomState = Random.Range(0, 3);
            
                if (!ingredient.productSO.isMergable)
                {
                    randomState = 0;
                }
            
                var recipeItem = new RecipeItem
                {
                    productState = ingredient.productState,
                    productLevel = randomState,
                    productSO = ingredient.productSO
                };
                recipeItems.Add(recipeItem);
            }

            return new Dish(nextOrderRecipe, recipeItems);
        }

        public List<Dish> GetActiveOrders()
        {
            return _activeOrders;
        }   
        
        public void CompleteOrder(Dish order)
        {
            if (_activeOrders.Contains(order))
            {
                _activeOrders.Remove(order);
                OnOrderComplete?.Invoke(this, new OnOrderCompleteEventArgs { completedOrder = order });
                spawnRecipeTimer = spawnRecipeTimerMax;
                PlayerTest.Instance.HandleObjectGive();
                Debug.Log(order.Name + " order completed!");
            }
        }

        public void RejectOrder(OrderRejectReason reason, Dish dish, OrderTable table)
        {
            OnOrderRejected?.Invoke(this, new OnOrderRejectedEventArgs { reason = reason, dish = dish, table = table });
        }    
    }
}