using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cozy
{
    // PROTOTYPE (cozy test branch): recipes open up step by step as the player serves orders
    [CreateAssetMenu(fileName = "CozyProgression", menuName = "Cozy/Progression")]
    public class CozyProgressionSO : ScriptableObject
    {
        [Serializable]
        public class Stage
        {
            public string stageName;

            [Tooltip("Total served orders needed to open this stage (0 = open from the start)")]
            [Min(0)] public int requiredOrders;

            [Tooltip("Recipes added to the menu by this stage")]
            public List<RecipeSO> recipes = new List<RecipeSO>();
        }

        [Tooltip("In the order they open")]
        public List<Stage> stages = new List<Stage>();
    }
}
