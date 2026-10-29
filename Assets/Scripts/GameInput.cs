using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput: MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    private MyInputs myInputs;
    private Camera mainCamera;

    // Reused for the UI check, so a click allocates nothing
    private readonly List<RaycastResult> uiHits = new List<RaycastResult>();

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

    private void OnDestroy()
    {
        if (myInputs == null) return;
        myInputs.Gameplay.Click.performed -= ClickOnPerformed;
        myInputs.Gameplay.CancelClick.performed -= CancelClickOnPerformed;
        myInputs.Disable();
        myInputs.Dispose();
    }

    private void CancelClickOnPerformed(InputAction.CallbackContext obj)
    {
        PlayerHand.Instance.CancelHolding();
    }

    private void ClickOnPerformed(InputAction.CallbackContext obj)
    {
        var screenPosition = GetMousePosition();

        // A click on a button or a panel of the screen UI must not reach the kitchen behind it
        if (IsOverScreenUI(screenPosition)) return;

        DetectObject(screenPosition);
    }

    // Only one object reacts: the one drawn on top among those that can be used with a hand.
    // Products, plates and cookware are not interactable, so a click passes through them to the table.
    // In 2D the order of ray hits says nothing about what is visually above,
    // so the objects are compared by their sprites' sorting (layer, then order in layer).
    // With equal sorting, the one whose centre is closer to the click wins.
    private void DetectObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D[] hitAll = Physics2D.GetRayIntersectionAll(ray);
        Vector2 clickPoint = mainCamera.ScreenToWorldPoint(screenPosition);

        IInteractable best = null;
        int bestLayer = int.MinValue;
        int bestOrder = int.MinValue;
        float bestDistance = float.MaxValue;

        foreach (var hit in hitAll)
        {
            if (hit.collider == null) continue;

            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) continue;

            GetSorting(hit.collider, out int layer, out int order);
            float distance = ((Vector2)hit.collider.bounds.center - clickPoint).sqrMagnitude;

            bool isAbove = layer > bestLayer
                           || (layer == bestLayer && order > bestOrder)
                           || (layer == bestLayer && order == bestOrder && distance < bestDistance);
            if (!isAbove) continue;

            best = interactable;
            bestLayer = layer;
            bestOrder = order;
            bestDistance = distance;
        }

        // The mouse always acts with the player's hand
        best?.Interact(PlayerHand.Instance);
    }

    // Sorting of the object's own sprite (on it or its children)
    private static void GetSorting(Collider2D collider, out int layer, out int order)
    {
        var sprite = collider.GetComponentInChildren<SpriteRenderer>();
        if (sprite == null)
        {
            layer = int.MinValue;
            order = int.MinValue;
            return;
        }
        layer = SortingLayer.GetLayerValueFromID(sprite.sortingLayerID);
        order = sprite.sortingOrder;
    }

    // True if the pointer is over the screen UI (HUD buttons, panels).
    // World-space canvases (icons and progress bars above tables and products) are ignored:
    // they sit right over the tables and must not block clicks on them.
    // EventSystem.IsPointerOverGameObject() can't tell them apart and is also unreliable
    // inside Input System callbacks, so the UI is raycast here directly.
    private bool IsOverScreenUI(Vector2 screenPosition)
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
