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
                ResourceManager.Instance.AddResource(product, 0, 5);
            }
        }

        private void HandleOrderComplete(object sender, OrderManager.OnOrderCompleteEventArgs e)
        {
            var activeOrders = OrderManager.Instance.GetActiveOrders();
        
            ClearProductSpawners();
            foreach (var activeOrder in activeOrders)
            {
                RespawnProductSpawners(activeOrder);
            }
        }
    
        private void RespawnProductSpawners(Dish order)
        {
            SpawnIfNotExists(order);
        }

        private void HandleNewRecipeSpawned(object sender, OrderManager.OnNewRecipeSpawnedEventArgs e)
        {
            SpawnIfNotExists(e.newOrder);
        }
    
        private void SpawnIfNotExists(Dish order)
        {
            foreach (var ingredient in order.ingredients)
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
