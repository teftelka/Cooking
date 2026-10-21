using System;
using UnityEngine;

namespace UIScripts
{
    public class InstrumentIconsUI : MonoBehaviour
    {
        [SerializeField] private CookingTool cookingTool;
        [SerializeField] private Transform iconTemplate;
    
        private void Start()
        {
            cookingTool.OnProductAdded += HandleProductAdded;
            cookingTool.OnEmptyTool += HandleEmptyTool;
        }

        private void OnDestroy()
        {
            if (cookingTool != null)
            {
                cookingTool.OnProductAdded -= HandleProductAdded;
                cookingTool.OnEmptyTool -= HandleEmptyTool;
            }
        }

        private void HandleEmptyTool(object sender, EventArgs e)
        {
            Rebuild();
        }

        private void HandleProductAdded(object sender, CookingTool.OnProductAddedEventArgs e)
        {
            Rebuild();
        }

        private void Rebuild()
        {
            // The template doubles as the "empty" placeholder: hidden while clones are made
            iconTemplate.gameObject.SetActive(false);
            int count = IconsListBuilder.Rebuild(transform, iconTemplate, IconsListBuilder.CollectItems(cookingTool.GetProducts()));
            if (count == 0)
                iconTemplate.gameObject.SetActive(true);
        }
    }
}
