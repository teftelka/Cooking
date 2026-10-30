using Interfaces;
using UnityEngine;

// The player's hand. What every hand can do is in IHand; on top of that the player's
// held object follows the mouse, and right-click sends it back to where it came from.
public class PlayerHand : MonoBehaviour, IHand, IZoneExitBlocker
{
    public static PlayerHand Instance { get; private set; }

    [SerializeField] private bool hasObject;
    [SerializeField] private BaseObject _product;
    [SerializeField] private Camera mainCamera;

    public bool HasObject => hasObject;
    public BaseObject HeldObject => _product;

    // The player can't go to another zone with an object in the hand
    public bool CanLeaveZone => !hasObject;

    private void Awake()
    {
        Instance = this;
        hasObject = false;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (hasObject)
        {
            MoveHeldObject();
        }
    }

    // Returns false if the hand is busy: the caller must not remove the object from its place
    public bool TryTake(BaseObject obj)
    {
        if (hasObject || obj == null) return false;

        hasObject = true;
        _product = obj;
        return true;
    }

    // Returns false if the hand was already empty
    public bool Release()
    {
        if (!hasObject) return false;

        ClearObject();
        return true;
    }

    private void ClearObject()
    {
        hasObject = false;
        _product = null;
    }

    private void MoveHeldObject()
    {
        Vector2 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

        worldPos.z = 0f;

        _product.transform.position = worldPos;
    }

    // Right-click: return the held object to its origin table
    public void CancelHolding()
    {
        if (!hasObject) return;

        // The origin table may refuse (occupied, or the object no longer fits there)
        if (_product.ReturnToOrigin())
            ClearObject();
    }
}
