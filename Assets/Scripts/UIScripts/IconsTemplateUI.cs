using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class IconsTemplateUI : MonoBehaviour
    {
        [SerializeField] private Image productIcon;
    
        public void SetIcon(Sprite icon)
        {
            productIcon.sprite = icon;
        }
    }
}
