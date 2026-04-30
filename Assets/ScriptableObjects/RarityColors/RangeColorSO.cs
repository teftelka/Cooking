using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RangeColorSO", menuName = "Scriptable Objects/RangeColorSO")]
    public class RangeColorSO : ScriptableObject
    {
        public int range;
        public Color color;
    }
}
