namespace Interfaces
{
    // Anything that holds one object and uses it on tables: the player's hand now, helpers later
    public interface IHand
    {
        bool HasObject { get; }
        BaseObject HeldObject { get; }

        // Takes the object. Returns false if the hand is busy:
        // the caller must not remove the object from its place then
        bool TryTake(BaseObject obj);

        // The held object has been given away: the hand becomes empty.
        // Returns false if it was already empty
        bool Release();
    }
}
