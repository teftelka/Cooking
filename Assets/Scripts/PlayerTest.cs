using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerTest: MonoBehaviour
    {
        public static PlayerTest Instance { get; set; }
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

        public void SetObject(BaseObject product)
        {
            hasObject = true;
            _product = product;
        }

        public BaseObject GetProduct()
        {
            return _product;
        }
        
        public void ClearObject()
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
        
        public void CancelHolding()
        {
            if (!hasObject) return;

            _product.ReturnToOrigin();
            ClearObject();
        }
    }
}