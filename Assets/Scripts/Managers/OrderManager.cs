using System;
using System.Collections.Generic;
using DefaultNamespace;
using Tables;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Managers
{
    public class OrderManager: MonoBehaviour
    {
        public static OrderManager Instance { get; private set; }
    
        [Tooltip("The menu: recipes that can be ordered. Unlock conditions will narrow it later")]
        [FormerlySerializedAs("recipeList")]
        [SerializeField] private RecipeListSO orderPool;

        [Tooltip("The cookbook: everything that can be assembled (the list the plate and the bases use). " +
                 "Only used to check that every recipe on the menu can actually be made")]
        [SerializeField] private RecipeListSO cookbook;

        [SerializeField] private List<Dish> _activeOrders;
        private float spawnRecipeTimer;
        private float spawnRecipeTimerMax = 3f;
        [SerializeField] private int maxActiveOrders = 2;
    
        public event EventHandler<OnNewRecipeSpawnedEventArgs> OnNewRecipeSpawned;
        public class OnNewRecipeSpawnedEventArgs : EventArgs
        {
            public Dish newOrder;
        }
    
        public event EventHandler<OnOrderCompleteEventArgs> OnOrderComplete;
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

#if UNITY_EDITOR
        // At Play too: the lists are separate assets and can change after this component was validated
        private void Start() => ValidateMenu();
        private void OnValidate() => ValidateMenu();

        private void ValidateMenu()
        {
            if (orderPool == null)
            {
                Debug.LogWarning($"{name}: Order Pool is empty, no orders will appear", this);
                return;
            }
            if (cookbook == null || cookbook == orderPool || orderPool.allRecipes == null) return;

            foreach (var recipe in orderPool.allRecipes)
            {
                if (recipe == null) continue;
                if (cookbook.allRecipes != null && cookbook.allRecipes.Contains(recipe)) continue;
                Debug.LogWarning($"{name}: '{recipe.name}' can be ordered but is not in the cookbook " +
                                 $"'{cookbook.name}', so it can't be assembled", this);
            }
        }
#endif
    
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
            var nextOrderRecipe = PickNextRecipe();
            if (nextOrderRecipe == null) return;

            var order = CreateOrder(nextOrderRecipe);
        
            OnNewRecipeSpawned?.Invoke(this, new OnNewRecipeSpawnedEventArgs { newOrder = order });
        
            _activeOrders.Add(order);
        }

        // The recipes that may be ordered right now.
        // The one place for future unlock conditions (level, opened products, day...)
        private List<RecipeSO> GetAvailableRecipes()
        {
            var available = new List<RecipeSO>();
            if (orderPool == null || orderPool.allRecipes == null) return available;

            foreach (var recipe in orderPool.allRecipes)
            {
                if (recipe != null) available.Add(recipe);
            }
            return available;
        }

        // The one place that decides which of the available recipes is ordered next
        private RecipeSO PickNextRecipe()
        {
            var available = GetAvailableRecipes();
            if (available.Count == 0) return null;
            return available[Random.Range(0, available.Count)];
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
                Debug.Log(order.Name + " order completed!");
            }
        }

        public void RejectOrder(OrderRejectReason reason, Dish dish, OrderTable table)
        {
            OnOrderRejected?.Invoke(this, new OnOrderRejectedEventArgs { reason = reason, dish = dish, table = table });
        }    
    }
}