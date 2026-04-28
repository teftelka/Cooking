using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeItem> ingredients;
    public GameObject resultPrefab;
    public string recipeName;
}
