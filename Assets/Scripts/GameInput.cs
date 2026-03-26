using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput: MonoBehaviour
{
    public static GameInput Instance { get; set; }
    
    private MyInputs myInputs;
    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        myInputs = new MyInputs();
        myInputs.Enable();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        myInputs.Gameplay.Click.performed += ClickOnPerformed;
        myInputs.Gameplay.CancelClick.performed += CancelClickOnPerformed;
    }

    private void CancelClickOnPerformed(InputAction.CallbackContext obj)
    {
        PlayerTest.Instance.CancelHolding();
    }

    private void ClickOnPerformed(InputAction.CallbackContext obj)
    {
        DetectObject();
    }

    private void DetectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(myInputs.Gameplay.Position.ReadValue<Vector2>());
        RaycastHit2D[] hitAll = Physics2D.GetRayIntersectionAll(ray);
        
        foreach (var hit in hitAll)
        {
            if (hit.collider != null)
            {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                clickable?.OnClick();
            }
        }
    }
    
    public Vector2 GetMousePosition()
    {
        return myInputs.Gameplay.Position.ReadValue<Vector2>();
    }
}
