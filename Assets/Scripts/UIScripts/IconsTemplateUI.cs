using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class IconsTemplateUI : MonoBehaviour
    {
        [SerializeField] private Image productIcon;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private ProductRangeColorsSO rangeColorsSO;
    
        public void SetIcon(Sprite icon, int range)
        {
            productIcon.sprite = icon;
            backgroundImage.GetComponent<Outline>().effectColor = rangeColorsSO._colors[range].color;
        }
    }
}
