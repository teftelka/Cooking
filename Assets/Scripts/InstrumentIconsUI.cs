using System;
using UnityEngine;

public class InstrumentIconsUI : MonoBehaviour
{
    [SerializeField] private CookingTool cookingTool;
    [SerializeField] private Transform iconTemplate;
    [SerializeField] private Sprite defaultIcon;
    
    private int iconsCount;
    
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
                child.GetComponent<IconsTemplateUI>().SetIcon(defaultIcon);
                continue;
            }
            Destroy(child.gameObject);
        }
        iconsCount = 0;
    }

    private void HandleProductAdded(object sender, CookingTool.OnProductAddedEventArgs e)
    {
        var sprite = e.product.GetSpriteRenderer().sprite;
        iconsCount++;
        UpdateVisual(sprite);
    }

    private void UpdateVisual(Sprite icon)
    {
        if (iconsCount == 1)
        {
            iconTemplate.GetComponent<IconsTemplateUI>().SetIcon(icon);
            return;
        }
        Transform newIcon = Instantiate(iconTemplate, transform);
        newIcon.GetComponent<IconsTemplateUI>().SetIcon(icon);
    }
}
