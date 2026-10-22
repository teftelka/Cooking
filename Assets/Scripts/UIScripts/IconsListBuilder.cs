using System.Collections.Generic;
using UnityEngine;

namespace UIScripts
{
    // Shared by the plate and cookware icon panels: redraws the whole list of
    // ingredient icons (a base counts with all its ingredients).
    public static class IconsListBuilder
    {
        public static int Rebuild(Transform container, Transform iconTemplate, IEnumerable<RecipeItem> items)
        {
            // Remove old icons. Hide first: Destroy happens only at the end of the frame
            foreach (Transform child in container)
            {
                if (child == iconTemplate) continue;
                child.gameObject.SetActive(false);
                Object.Destroy(child.gameObject);
            }

            int count = 0;
            foreach (var item in items)
            {
                if (item.productSO == null) continue;

                Transform newIcon = Object.Instantiate(iconTemplate, container);
                newIcon.GetComponent<IconsTemplateUI>().SetIcon(item.productSO.icon, item.productLevel);
                newIcon.gameObject.SetActive(true);
                count++;
            }
            return count;
        }

        public static IEnumerable<RecipeItem> CollectItems(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                if (product == null) continue;
                foreach (var item in product.GetRecipeItems())
                    yield return item;
            }
        }
    }
}
