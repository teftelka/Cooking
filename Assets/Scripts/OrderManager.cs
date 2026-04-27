using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager: MonoBehaviour
{
    public static OrderManager Instance { get; set; }
    
    [SerializeField] private RecipeListSO recipeList;
    private List<RecipeSO> _activeOrders;
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
        OnNewRecipeSpawned?.Invoke(this, new OnNewRecipeSpawnedEventArgs { newRecipe = nextOrderRecipe });
        
        _activeOrders.Add(nextOrderRecipe);
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
            OnOrderComplete?.Invoke(this, new OnOrderCompleteEventArgs { recipeConplete = recipe });
            Debug.Log(recipe.name + " order completed!");
        }
    }    
}