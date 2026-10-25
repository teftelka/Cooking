using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerTest: MonoBehaviour
    {
        public static PlayerTest Instance { get; private set; }
        [SerializeField] private bool hasObject;
        [SerializeField] private BaseObject _product;
        [SerializeField] private Camera mainCamera;

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
        
        public bool HasObject()
        {
            return hasObject;
        }

        private void SetObject(BaseObject product)
        {
            hasObject = true;
            _product = product;
        }

        public BaseObject GetProduct()
        {
            return _product;
        }
        
        private void ClearObject()
        {
            hasObject = false;
            _product = null;
        }
        
        // Returns false if the hand was already empty
        public bool HandleObjectGive()
        {
            if (!hasObject) return false;

            ClearObject();
            return true;
        }
        
        // Returns false if the hand is busy: the caller must not remove the object from its place
        public bool HandleObjectTake(BaseObject product)
        {
            if (hasObject || product == null) return false;
            SetObject(product);
            return true;
        }
        
        private void MoveHeldObject()
        {
            Vector2 mousePos = GameInput.Instance.GetMousePosition();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            worldPos.z = 0f;

            _product.transform.position = worldPos;
        }
        
        public void CancelHolding()
        {
            if (!hasObject) return;
            
            // The origin table may refuse (occupied, or the object no longer fits there)
            if (_product.ReturnToOrigin())
                ClearObject();
        }
    }
}