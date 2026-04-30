using System;
using UIScripts;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private Plate plate;
    [SerializeField] private Transform iconTemplate;
    
    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }
    
    private void Start()
    {
        plate.OnProductAdded += HandleProductAdded;
        plate.OnEmptyPlate += HandleEmptyPlate;
    }

    private void HandleEmptyPlate(object sender, EventArgs e)
    {
        foreach (Transform child in transform) 
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }
    }
    
    private void HandleProductAdded(object sender, Plate.OnProductAddedEventArgs e)
    {
        var sprite = e.product.GetDefaultSprite();
        UpdateVisual(sprite, e.product.GetProductRange());
    }

    private void UpdateVisual(Sprite icon, int range)
    {
        Transform newIcon = Instantiate(iconTemplate, transform);
        newIcon.GetComponent<IconsTemplateUI>().SetIcon(icon, range);
        newIcon.gameObject.SetActive(true);
    }
}
