using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Products;
using Tables;
using UnityEngine;

namespace Managers
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { get; set; }
    
        [SerializeField] private GameObject spawnerPrefab;
        [SerializeField] private ProductList productsToSpawn;
    
        [SerializeField] private Transform firstSpawnPoint;
        [SerializeField] private float offsetX = 2.5f;
    
        private List<GameObject> spawners = new List<GameObject>();

        private void Awake()
        {
            Instance = this;
        }
    
        private void Start()
        {
            OrderManager.Instance.OnNewRecipeSpawned += HandleNewRecipeSpawned;
            OrderManager.Instance.OnOrderComplete += HandleOrderComplete;
        
        
            foreach (var product in productsToSpawn.allRawProducts)
            {
                ResourceManager.Instance.Add(product, 5);
            }
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            var activeRecipes = OrderManager.Instance.GetActiveOrders();
        
            ClearProductSpawners();
            foreach (var activeRecipe in activeRecipes)
            {
                RespawnProductSpawners(activeRecipe);
            }
        }
    
        private void RespawnProductSpawners(RecipeSO recipe)
        {
            SpawnIfNotExists(recipe);
        }

        private void HandleNewRecipeSpawned(object sender, OrderManager.OnNewRecipeSpawnedEventArgs e)
        {
            SpawnIfNotExists(e.newRecipe);
        }
    
        private void SpawnIfNotExists(RecipeSO recipe)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                bool exists = spawners.Any(s =>
                    s.GetComponent<SpawnTable>().GetProductSO() == ingredient.productSO);

                if (!exists)
                    SpawnProductSpawner(ingredient.productSO);
            }
        }
    
        private void ClearProductSpawners()
        {
            foreach (var spawner in spawners)
            {
                Destroy(spawner);
            }
            spawners.Clear();
        }

        private void SpawnProductSpawner(ProductSO productSO)
        {
            Vector3 spawnPos = firstSpawnPoint.position + Vector3.right * (offsetX * spawners.Count);

            var newSpawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.identity);
            spawners.Add(newSpawner);
        
            newSpawner.GetComponent<SpawnTable>().SetProductSO(productSO);
        }
    }
}
