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

        // With an object in the hand the zone buttons can't be pressed
        private void Update()
        {
            bool canLeave = AreaManager.Instance == null || AreaManager.Instance.CanLeaveCurrentArea;
            SetInteractable(toKitchenButton, canLeave);
            SetInteractable(toGardenButton, canLeave);
            SetInteractable(toPondButton, canLeave);
        }

        private static void SetInteractable(Button button, bool interactable)
        {
            if (button != null && button.interactable != interactable)
                button.interactable = interactable;
        }
    }
}