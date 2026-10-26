using UnityEngine;

// Product levels (rarity): 0 is the base level, Max is the last one.
// A product of the last level can't be merged and gains no more experience.
// ProductRangeColorsSO needs one color per level (Max + 1 colors).
public static class ProductLevels
{
    public const int Max = 4;

    public static int Clamp(int level) => Mathf.Clamp(level, 0, Max);
}
