using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ProductRangeColors", menuName = "Scriptable Objects/ProductRangeColors")]
    public class ProductRangeColorsSO : ScriptableObject
    {
        public List<RangeColorSO> _colors;
    }
}
