using System;
using UnityEngine;

namespace UIScripts
{
    // Icon panel above a product: its own icon, and for a base also all its ingredients.
    // Lives in a separate panel prefab nested into every product prefab; finds its Product in the parents.
    public class ProductIconsUI : MonoBehaviour
    {
        [SerializeField] private Transform iconTemplate;

        [Tooltip("Show the icon also while the product is still in its start state (e.g. a whole raw tomato)")]
        [SerializeField] private bool showInStartState = false;

        private Product product;

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);

            // Subscribe in Awake: Product sets its start state in its own Start and raises OnStateChanged
            product = GetComponentInParent<Product>();
            if (product == null)
            {
                Debug.LogError($"{nameof(ProductIconsUI)} must be placed under a Product", this);
                return;
            }

            product.OnStateChanged += HandleProductChanged;
            product.OnContentsChanged += HandleProductChanged;
            product.OnLevelChanged += HandleProductChanged;
            product.OnContainmentChanged += HandleProductChanged;
        }

        private void Start()
        {
            Refresh();
        }

        private void OnDestroy()
        {
            if (product == null) return;

            product.OnStateChanged -= HandleProductChanged;
            product.OnContentsChanged -= HandleProductChanged;
            product.OnLevelChanged -= HandleProductChanged;
            product.OnContainmentChanged -= HandleProductChanged;
        }

        private void HandleProductChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (product == null) return;

            bool visible = ShouldBeVisible();
            gameObject.SetActive(visible);
            if (!visible) return;

            IconsListBuilder.Rebuild(transform, iconTemplate, product.GetRecipeItems());
        }

        private bool ShouldBeVisible()
        {
            // Inside a plate or cookware their own panel shows the product
            if (product.IsInsideContainer) return false;

            // A base with ingredients always shows what is on it
            if (product.HasContents) return true;

            if (showInStartState) return true;

            var state = product.GetCurrentState();
            var startState = product.GetProductData() != null ? product.GetProductData().startState : null;
            return state != null && state != startState;
        }
    }
}
