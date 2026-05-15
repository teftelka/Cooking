using System;
using UnityEngine;

namespace Managers
{
    public class GardenPlateManager: MonoBehaviour
    {
        [SerializeField] private GameObject gardenPlatePrefab;
        [SerializeField] private Transform firstSpawnPoint;
        [SerializeField] private ProductSO productSO;
        
        public static GardenPlateManager Instance { get; private  set; }
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SpawnGardenPlate(productSO);
        }


        public void SpawnGardenPlate(ProductSO product)
        {
            var newGardenPlate = Instantiate(gardenPlatePrefab, firstSpawnPoint.position, Quaternion.identity);
            newGardenPlate.GetComponent<GardenPlate>().SetupGardenPlate(product);
        }
    }
}