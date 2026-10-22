using System.Collections.Generic;

namespace Recipes
{
    /// <summary>
    /// Single place that decides how a set of ingredients relates to a recipe.
    /// Level rules:
    ///  - while assembling a dish, levels are ignored (product + state only);
    ///  - when serving an order, each dish ingredient must be at least the required level.
    /// </summary>
    public static class RecipeMatcher
    {
        private static bool SameIngredient(RecipeItem a, RecipeItem b)
        {
            return a.productSO == b.productSO && a.productState == b.productState;
        }

        /// <summary>Every item has its own matching ingredient in the recipe (levels ignored).</summary>
        public static bool IsSubsetOf(IReadOnlyList<RecipeItem> items, RecipeSO recipe)
        {
            if (recipe == null || recipe.ingredients == null) return false;
            if (items.Count > recipe.ingredients.Count) return false;

            var remaining = new List<RecipeItem>(recipe.ingredients);
            foreach (var item in items)
            {
                int index = remaining.FindIndex(r => SameIngredient(r, item));
                if (index == -1) return false;
                remaining.RemoveAt(index);
            }
            return true;
        }

        /// <summary>Items match the recipe exactly: same count and same ingredients (levels ignored).</summary>
        public static bool IsExactly(IReadOnlyList<RecipeItem> items, RecipeSO recipe)
        {
            if (recipe == null || recipe.ingredients == null) return false;
            return items.Count == recipe.ingredients.Count && IsSubsetOf(items, recipe);
        }

        /// <summary>Items can still become some recipe from the list.</summary>
        public static bool FitsAnyRecipe(IReadOnlyList<RecipeItem> items, RecipeListSO list)
        {
            if (list == null || list.allRecipes == null) return false;
            foreach (var recipe in list.allRecipes)
            {
                if (IsSubsetOf(items, recipe)) return true;
            }
            return false;
        }

        /// <summary>
        /// Fillings of a base can still become a recipe that contains this base product.
        /// The base is matched by product only: its state changes later (dough is cut while
        /// assembling and baked afterwards), so the final state is checked when the dish is served.
        /// </summary>
        public static bool FitsAnyRecipeWithBase(ProductSO baseProduct, IReadOnlyList<RecipeItem> fillings, RecipeListSO list)
        {
            if (baseProduct == null || list == null || list.allRecipes == null) return false;
            foreach (var recipe in list.allRecipes)
            {
                if (FitsRecipeWithBase(baseProduct, fillings, recipe)) return true;
            }
            return false;
        }

        private static bool FitsRecipeWithBase(ProductSO baseProduct, IReadOnlyList<RecipeItem> fillings, RecipeSO recipe)
        {
            if (recipe == null || recipe.ingredients == null) return false;

            var remaining = new List<RecipeItem>(recipe.ingredients);
            int baseIndex = remaining.FindIndex(r => r.productSO == baseProduct);
            if (baseIndex == -1) return false;
            remaining.RemoveAt(baseIndex);

            if (fillings.Count > remaining.Count) return false;
            foreach (var item in fillings)
            {
                int index = remaining.FindIndex(r => SameIngredient(r, item));
                if (index == -1) return false;
                remaining.RemoveAt(index);
            }
            return true;
        }

        /// <summary>First recipe from the list that the items form exactly, or null.</summary>
        public static RecipeSO FindExactRecipe(IReadOnlyList<RecipeItem> items, RecipeListSO list)
        {
            if (list == null || list.allRecipes == null) return null;
            foreach (var recipe in list.allRecipes)
            {
                if (IsExactly(items, recipe)) return recipe;
            }
            return null;
        }

        /// <summary>
        /// The dish fulfils the order: same number of ingredients, same products and states,
        /// and every dish ingredient is at least the level the order requires.
        /// </summary>
        public static bool SatisfiesOrder(IReadOnlyList<RecipeItem> dish, IReadOnlyList<RecipeItem> order)
        {
            if (dish == null || order == null) return false;
            if (dish.Count != order.Count) return false;

            // Most demanding requirements first, and each one takes the weakest dish item
            // that still satisfies it. This keeps high-level items for high requirements
            // when a recipe has the same product more than once.
            var requirements = new List<RecipeItem>(order);
            requirements.Sort((a, b) => b.productLevel.CompareTo(a.productLevel));

            var available = new List<RecipeItem>(dish);
            foreach (var required in requirements)
            {
                int best = -1;
                for (int i = 0; i < available.Count; i++)
                {
                    var candidate = available[i];
                    if (!SameIngredient(candidate, required)) continue;
                    if (candidate.productLevel < required.productLevel) continue;
                    if (best == -1 || candidate.productLevel < available[best].productLevel)
                        best = i;
                }

                if (best == -1) return false;
                available.RemoveAt(best);
            }
            return true;
        }
    }
}
