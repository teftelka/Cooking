using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerTableUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI productAmountText;
    
    private void Start()
    {
        ResourceManager.Instance.OnResourceChanged += HandleResourceChanged;
    }

    private void HandleResourceChanged(object sender, ResourceManager.OnResourceChangedEventArgs e)
    {
        UpdateProductAmount(e.newAmount);
    }
    
    private void UpdateProductAmount(int amount)
    {
        productAmountText.text = amount.ToString();
    }
    
    public void SetImage(Sprite newImage)
    {
        image.sprite = newImage;
    }
}
