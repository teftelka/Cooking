using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerTableUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI productAmountText;
    [SerializeField] private TextMeshProUGUI priceText;

    public void UpdateProductAmount(int amount)
    {
        productAmountText.text = amount.ToString();
    }
    
    public void SetSpawnerInfo(Sprite sprite, int price)
    {
        image.sprite = sprite;
        priceText.text = price.ToString();
    }
    
}
