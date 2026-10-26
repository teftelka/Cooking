using System;
using Interfaces;
using UnityEngine;

namespace UIScripts
{
    // Icon panel of a container (plate, pan, pot, baking sheet): the icons of everything inside,
    // a base counts with all its ingredients. Finds its container in the parents.
    public class ContainerIconsUI : MonoBehaviour
    {
        [SerializeField] private Transform iconTemplate;

        [Tooltip("Show the template as an 'empty' placeholder while nothing is inside (cookware). Off for the plate")]
        [SerializeField] private bool showTemplateWhenEmpty = true;

        private IProductContainer container;

        private void Awake()
        {
            iconTemplate.gameObject.SetActive(false);

            container = GetComponentInParent<IProductContainer>();
            if (container == null)
            {
                Debug.LogError($"{nameof(ContainerIconsUI)} must be placed under a plate or cookware", this);
                return;
            }
            container.OnContentsChanged += HandleContentsChanged;
        }

        private void Start()
        {
            Rebuild();
        }

        private void OnDestroy()
        {
            if (container != null)
                container.OnContentsChanged -= HandleContentsChanged;
        }

        private void HandleContentsChanged(object sender, EventArgs e)
        {
            Rebuild();
        }

        // Redrawn from the full contents on every change: ingredients can be added
        // to a base that is already inside, without a new product arriving
        private void Rebuild()
        {
            if (container == null) return;

            iconTemplate.gameObject.SetActive(false);
            int count = IconsListBuilder.Rebuild(transform, iconTemplate, IconsListBuilder.CollectItems(container.GetProducts()));
            if (count == 0 && showTemplateWhenEmpty)
                iconTemplate.gameObject.SetActive(true);
        }
    }
}
