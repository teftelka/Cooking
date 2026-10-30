using Interfaces;
using UnityEngine;

// The kitchen's mouse controls: a click uses the player's hand on the table under the cursor,
// right-click sends the held object back where it came from.
// Put it into the kitchen GameZone's "Active When Shown": then it works only while the kitchen is on screen.
public class KitchenInput : MonoBehaviour
{
    [Tooltip("Empty: the scene's PlayerHand")]
    [SerializeField] private PlayerHand hand;

    [Tooltip("Empty: the main camera")]
    [SerializeField] private Camera viewCamera;

    private GameInput subscribedTo;

    private PlayerHand Hand => hand != null ? hand : PlayerHand.Instance;
    private Camera ViewCamera => viewCamera != null ? viewCamera : Camera.main;

    // GameInput lives in another scene and may wake up later, so try on enable and again on start
    private void OnEnable() => Subscribe();
    private void Start() => Subscribe();
    private void OnDisable() => Unsubscribe();

    private void Subscribe()
    {
        if (subscribedTo != null || GameInput.Instance == null) return;
        subscribedTo = GameInput.Instance;
        subscribedTo.OnPointerDown += HandlePointerDown;
        subscribedTo.OnCancel += HandleCancel;
    }

    private void Unsubscribe()
    {
        if (subscribedTo == null) return;
        subscribedTo.OnPointerDown -= HandlePointerDown;
        subscribedTo.OnCancel -= HandleCancel;
        subscribedTo = null;
    }

    private void HandleCancel(Vector2 screenPosition)
    {
        if (Hand != null) Hand.CancelHolding();
    }

    // Only one object reacts: the one drawn on top among those that can be used with a hand.
    // Products, plates and cookware are not interactable, so a click passes through them to the table.
    // In 2D the order of ray hits says nothing about what is visually above,
    // so the objects are compared by their sprites' sorting (layer, then order in layer).
    // With equal sorting, the one whose centre is closer to the click wins.
    private void HandlePointerDown(Vector2 screenPosition)
    {
        var cam = ViewCamera;
        if (cam == null || Hand == null) return;

        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit2D[] hitAll = Physics2D.GetRayIntersectionAll(ray);
        Vector2 clickPoint = cam.ScreenToWorldPoint(screenPosition);

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

        best?.Interact(Hand);
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
}
