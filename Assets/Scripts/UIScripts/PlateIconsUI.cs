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
        // Redraw from the full contents on every change: ingredients can be added
        // to a base that is already on the plate, without a new product arriving
        plate.OnContentsChanged += HandleContentsChanged;
        Rebuild();
    }

    private void OnDestroy()
    {
        if (plate != null)
            plate.OnContentsChanged -= HandleContentsChanged;
    }

    private void HandleContentsChanged(object sender, EventArgs e)
    {
        Rebuild();
    }

    private void Rebuild()
    {
        IconsListBuilder.Rebuild(transform, iconTemplate, IconsListBuilder.CollectItems(plate.GetProducts()));
    }
}
