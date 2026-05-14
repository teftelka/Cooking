using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class OrderManager: MonoBehaviour
    {
        public static OrderManager Instance { get; set; }
    
        [SerializeField] private RecipeListSO recipeList;
        [SerializeField] private List<RecipeSO> _activeOrders;
        private float spawnRecipeTimer;
        private float spawnRecipeTimerMax = 3f;
        [SerializeField] private int maxActiveOrders = 2;
    
        public EventHandler<OnNewRecipeSpawnedEventArgs> OnNewRecipeSpawned;
        public class OnNewRecipeSpawnedEventArgs : EventArgs
        {
            public RecipeSO newRecipe;
        }
    
        public EventHandler<OnOrderCompleteEventArgs> OnOrderComplete;
        public class OnOrderCompleteEventArgs : EventArgs
        {
            public RecipeSO recipeConplete;
        }
    
        private void Awake()
        {
            Instance = this;
            _activeOrders = new List<RecipeSO>();
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
            var modifiedRecipeSO = RecipeModification(nextOrderRecipe);
        
            OnNewRecipeSpawned?.Invoke(this, new OnNewRecipeSpawnedEventArgs { newRecipe = modifiedRecipeSO });
        
            _activeOrders.Add(modifiedRecipeSO);
        }

        private RecipeSO RecipeModification(RecipeSO nextOrderRecipe)
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

            RecipeSO modifiedRecipeSO = ScriptableObject.CreateInstance<RecipeSO>();
        
            modifiedRecipeSO.ingredients = recipeItems;
            modifiedRecipeSO.resultPrefab = nextOrderRecipe.resultPrefab;
            modifiedRecipeSO.recipeName = nextOrderRecipe.recipeName;
        
            return modifiedRecipeSO;
        }

        public List<RecipeSO> GetActiveOrders()
        {
            return _activeOrders;
        }   
        
        public void CompleteOrder(RecipeSO recipe)
        {
            if (_activeOrders.Contains(recipe))
            {
                _activeOrders.Remove(recipe);
                recipeList.allRecipes.Remove(recipe);
                OnOrderComplete?.Invoke(this, new OnOrderCompleteEventArgs { recipeConplete = recipe });
                spawnRecipeTimer = spawnRecipeTimerMax;
                PlayerTest.Instance.HandleObjectGive();
                Debug.Log(recipe.name + " order completed!");
            }
        }    
    }
}