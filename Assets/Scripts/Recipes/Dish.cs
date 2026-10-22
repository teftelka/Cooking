using System;
using System.Collections.Generic;

// A concrete dish: an order (ingredients with the levels it requires) or what is on a plate
// (ingredients with their actual levels). The RecipeSO asset stays a template and is never
// copied at runtime: many orders can share one recipe with different levels.
[Serializable]
public class Dish
{
    public RecipeSO recipe;
    public List<RecipeItem> ingredients = new List<RecipeItem>();

    public Dish() { }

    public Dish(RecipeSO recipe, List<RecipeItem> ingredients)
    {
        this.recipe = recipe;
        this.ingredients = ingredients ?? new List<RecipeItem>();
    }

    public string Name => recipe != null ? recipe.recipeName : string.Empty;
}
