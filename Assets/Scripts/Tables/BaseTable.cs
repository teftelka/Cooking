using Interaction;
using Interfaces;
using UnityEngine;

namespace Tables
{
    public class BaseTable: MonoBehaviour, IInteractable
    {
        [SerializeField] protected GameObject spawnPosition;
    
        [SerializeField] protected  bool _hasObject;
        [SerializeField] protected  BaseObject _objectOnTable;

        public bool HasObject => _hasObject;
        public BaseObject ObjectOnTable => _objectOnTable;

        // Default: shared rules from InteractionResolver, with whatever hand is used.
        // Special tables (orders, sink, bin, plate stack, merge) override this.
        public virtual void Interact(IHand hand)
        {
            InteractionResolver.Resolve(this, hand);
        }

        // ---- Table policy ----
        public virtual bool CanPlace(BaseObject obj) => true;
        public virtual bool CanTake() => true;
        public virtual bool AllowSwap => false;
        // Right-click return to this table; by default the same as a normal placement
        public virtual bool CanReturn(BaseObject obj) => CanPlace(obj);

        // ---- Reactions ----
        protected virtual void OnPlaced(BaseObject obj) { }
        protected virtual void OnTaken(BaseObject obj) { }

        public virtual void SetObjectOnTable(BaseObject _object)
        {
            _hasObject = true;
            _objectOnTable = _object;
            _object.transform.position = spawnPosition.transform.position;
            _object.RememberOrigin(this);
            OnPlaced(_object);
        }

        // What an empty hand gets when clicking this table. By default: the object on it.
        // A station with a fixed tool gives the tool's contents instead.
        public virtual BaseObject TakeForEmptyHand()
        {
            if (!_hasObject || !CanTake()) return null;
            return TakeObject();
        }

        // Removes the object from the table and notifies the table
        public BaseObject TakeObject()
        {
            var taken = GiveObject();
            if (taken != null) OnTaken(taken);
            return taken;
        }

        public virtual bool TryReturnObject(BaseObject obj)
        {
            if (_hasObject) return false;
            if (!CanReturn(obj)) return false;

            SetObjectOnTable(obj);
            return true;
        }
        
        protected virtual BaseObject GiveObject()
        {
            BaseObject objectOnTable = _objectOnTable;
            _hasObject = false;
            _objectOnTable = null;
            return objectOnTable;
        }
    }
}
