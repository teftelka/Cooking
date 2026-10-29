using System.Collections.Generic;
using UnityEngine;

namespace Levels
{
    // PROTOTYPE (levels test branch): one timed shift with its own menu
    [CreateAssetMenu(fileName = "Level", menuName = "Levels/Level")]
    public class LevelSO : ScriptableObject
    {
        public string levelName;

        [Tooltip("Shift length in seconds")]
        [Min(10)] public float duration = 180f;

        [Tooltip("Recipes ordered on this level. They must also be in the cookbook (the plate's list)")]
        public List<RecipeSO> recipes = new List<RecipeSO>();

        [Tooltip("Orders at the same time. 0 = keep the OrderManager value")]
        [Min(0)] public int maxActiveOrders = 0;

        [Tooltip("Orders may ask for upgraded (merged) ingredients")]
        public bool randomIngredientLevels = false;

        [Tooltip("Completed orders needed for 1, 2 and 3 stars")]
        public List<int> starThresholds = new List<int> { 3, 5, 7 };

        public int GetStars(int completedOrders)
        {
            int stars = 0;
            foreach (var threshold in starThresholds)
            {
                if (completedOrders >= threshold) stars++;
            }
            return stars;
        }
    }
}
