using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerTableUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI productAmountText;

    public void UpdateProductAmount(int amount)
    {
        productAmountText.text = amount.ToString();
    }
    
    public void SetProductSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
