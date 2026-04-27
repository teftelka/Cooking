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

        private void HandleEmptyTool(object sender, EventArgs e)
        {
            foreach (Transform child in transform)
            {
                if (child == iconTemplate)
                {
                    iconTemplate.gameObject.SetActive(true);
                    continue;
                }
                Destroy(child.gameObject);
            }
        }

        private void HandleProductAdded(object sender, CookingTool.OnProductAddedEventArgs e)
        {
            iconTemplate.gameObject.SetActive(false);
            var sprite = e.product.GetSpriteRenderer().sprite;
            UpdateVisual(sprite);
        }

        private void UpdateVisual(Sprite icon)
        {
            Transform newIcon = Instantiate(iconTemplate, transform);
            newIcon.GetComponent<IconsTemplateUI>().SetIcon(icon);
            newIcon.gameObject.SetActive(true);
        }
    }
}
