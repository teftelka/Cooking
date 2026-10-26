using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ProductRangeColors", menuName = "Scriptable Objects/ProductRangeColors")]
    public class ProductRangeColorsSO : ScriptableObject
    {
        public List<RangeColorSO> _colors;

        // One color per product level (0..ProductLevels.Max). A missing color never crashes the UI:
        // the last available one is used
        public Color GetColor(int level)
        {
            if (_colors == null || _colors.Count == 0) return Color.white;
            int index = Mathf.Clamp(level, 0, _colors.Count - 1);
            return _colors[index] != null ? _colors[index].color : Color.white;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            int needed = ProductLevels.Max + 1;
            if (_colors == null || _colors.Count < needed)
                Debug.LogWarning($"{name}: {needed} colors are needed (levels 0..{ProductLevels.Max}), " +
                                 $"there are {(_colors == null ? 0 : _colors.Count)}", this);
        }
#endif
    }
}
