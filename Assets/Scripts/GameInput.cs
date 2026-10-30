using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Reads the mouse and reports plain facts: pressed here, released here, right button.
// Knows nothing about tables, hands or garden beds: each zone has its own input component
// (KitchenInput, later the garden's and the pond's) that listens to these events
// and decides what they mean there. Lives in the core scene.
public class GameInput: MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    // Left button pressed outside the screen UI (screen position)
    public event Action<Vector2> OnPointerDown;
    // Left button released, after a press that was reported (so a drag, e.g. a slingshot, can end anywhere)
    public event Action<Vector2> OnPointerUp;
    // Right button
    public event Action<Vector2> OnCancel;

    private MyInputs myInputs;
    private bool pressReported;

    // Reused for the UI check, so a click allocates nothing
    private readonly List<RaycastResult> uiHits = new List<RaycastResult>();

    private void Awake()
    {
        Instance = this;
        myInputs = new MyInputs();
        myInputs.Enable();
    }

    private void Start()
    {
        myInputs.Gameplay.Click.performed += ClickOnPerformed;
        myInputs.Gameplay.Click.canceled += ClickOnCanceled;
        myInputs.Gameplay.CancelClick.performed += CancelClickOnPerformed;
    }

    private void OnDestroy()
    {
        if (myInputs == null) return;
        myInputs.Gameplay.Click.performed -= ClickOnPerformed;
        myInputs.Gameplay.Click.canceled -= ClickOnCanceled;
        myInputs.Gameplay.CancelClick.performed -= CancelClickOnPerformed;
        myInputs.Disable();
        myInputs.Dispose();
    }

    private void ClickOnPerformed(InputAction.CallbackContext obj)
    {
        var screenPosition = GetMousePosition();

        // A press on a button or a panel of the screen UI (or on the dark screen during a zone switch)
        // must not reach the zone behind it
        if (IsOverScreenUI(screenPosition)) return;

        pressReported = true;
        OnPointerDown?.Invoke(screenPosition);
    }

    private void ClickOnCanceled(InputAction.CallbackContext obj)
    {
        if (!pressReported) return;
        pressReported = false;
        OnPointerUp?.Invoke(GetMousePosition());
    }

    private void CancelClickOnPerformed(InputAction.CallbackContext obj)
    {
        OnCancel?.Invoke(GetMousePosition());
    }

    // True if the pointer is over the screen UI (HUD buttons, panels, the zone-switch fade).
    // World-space canvases (icons and progress bars above tables and products) are ignored:
    // they sit right over the tables and must not block clicks on them.
    // EventSystem.IsPointerOverGameObject() can't tell them apart and is also unreliable
    // inside Input System callbacks, so the UI is raycast here directly.
    public bool IsOverScreenUI(Vector2 screenPosition)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        var pointerData = new PointerEventData(eventSystem) { position = screenPosition };
        uiHits.Clear();
        eventSystem.RaycastAll(pointerData, uiHits);

        foreach (var uiHit in uiHits)
        {
            var canvas = uiHit.gameObject.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.rootCanvas.renderMode != RenderMode.WorldSpace)
                return true;
        }
        return false;
    }
    
    public Vector2 GetMousePosition()
    {
        return myInputs.Gameplay.Position.ReadValue<Vector2>();
    }
}
