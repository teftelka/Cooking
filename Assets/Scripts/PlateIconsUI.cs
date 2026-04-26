using System;
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
