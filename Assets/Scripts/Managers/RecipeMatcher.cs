using System.Collections.Generic;

namespace Managers
{
    public static class RecipeMatcher
    {
        public static bool IsSubset(
            IReadOnlyList<RecipeItem> items,
            RecipeSO recipe)
        {
            List<RecipeItem> remaining = new(recipe.ingredients);

            foreach (var item in items)
            {
                int index = remaining.FindIndex(r => Matches(r, item));

                if (index == -1)
                    return false;

                remaining.RemoveAt(index);
            }

            return true;
        }

        public static bool IsExact(
            IReadOnlyList<RecipeItem> items,
            RecipeSO recipe)
        {
            return items.Count == recipe.ingredients.Count &&
                   IsSubset(items, recipe);
        }

        private static bool Matches(RecipeItem a, RecipeItem b)
        {
            return a.productSO == b.productSO &&
                   a.productState == b.productState &&
                   a.productLevel == b.productLevel;
        }
    }
}