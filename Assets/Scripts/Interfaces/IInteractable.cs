namespace Interfaces
{
    // Something used with a hand: tables, spawners. The mouse calls it with the player's hand
    public interface IInteractable
    {
        void Interact(IHand hand);
    }
}
