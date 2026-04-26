using UnityEngine;
using UnityEngine.UI;

public class IconsTemplateUI : MonoBehaviour
{
    [SerializeField] private Image productIcon;
    
    public void SetIcon(Sprite icon)
    {
        productIcon.sprite = icon;
    }
}
