using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class MainUI: MonoBehaviour
    {
        [SerializeField] private Button toKitchenButton;
        [SerializeField] private Button toGardenButton;
        [SerializeField] private Button toPondButton;

        private void Awake()
        {
            toKitchenButton.onClick.AddListener(() => 
                AreaManager.Instance.ShowArea(GameAreaType.Kitchen));
            
            toGardenButton.onClick.AddListener(() => 
                AreaManager.Instance.ShowArea(GameAreaType.Garden));
            
            toPondButton.onClick.AddListener(() => 
                AreaManager.Instance.ShowArea(GameAreaType.Pond));
            

        }
    }
}