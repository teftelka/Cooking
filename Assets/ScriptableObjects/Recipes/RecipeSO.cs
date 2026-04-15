using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeItem> ingredients;
    public GameObject resultPrefab;
}
